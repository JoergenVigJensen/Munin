﻿// Modified from Douglas Crockford's cycle.js (https://github.com/douglascrockford/JSON-js/blob/master/cycle.js)
// Ported over to TypeScript, and modified to handle the reference schema that Json.NET uses.
var JsonNetDecycle;
(function (JsonNetDecycle) {
    function findReferences(obj, catalog) {
        // The catalogObject function walks recursively through an object graph
        // looking for $id properties. When it finds an object with that property, then
        // it adds it to the catalog under that key.
        var i;
        if (obj && typeof obj === "object") {
            var id = obj.$id;
            if (typeof id === "string") {
                catalog[id] = obj;
            }
            if (Object.prototype.toString.apply(obj) === "[object Array]") {
                for (i = 0; i < obj.length; i += 1) {
                    findReferences(obj[i], catalog);
                }
            }
            else {
                for (name in obj) {
                    if (obj.hasOwnProperty(name)) {
                        if (typeof obj[name] === "object") {
                            findReferences(obj[name], catalog);
                        }
                    }
                }
            }
        }
    }
    function resolveReferences(obj, catalog) {
        var i, item, name, id;
        if (obj && typeof obj === "object") {
            if (Object.prototype.toString.apply(obj) === "[object Array]") {
                for (i = 0; i < obj.length; i += 1) {
                    item = obj[i];
                    if (item && typeof item === "object") {
                        id = item.$ref;
                        if (typeof id === "string") {
                            obj[i] = catalog[id];
                        }
                        else {
                            obj[i] = resolveReferences(item, catalog);
                        }
                    }
                }
            }
            else if (obj.$values && Object.prototype.toString.apply(obj.$values) === "[object Array]") {
                var arr = new Array();
                for (i = 0; i < obj.$values.length; i += 1) {
                    item = obj.$values[i];
                    if (item && typeof item === "object") {
                        id = item.$ref;
                        if (typeof id === "string") {
                            arr[i] = catalog[id];
                        }
                        else {
                            arr[i] = resolveReferences(item, catalog);
                        }
                    }
                    else {
                        arr[i] = item;
                    }
                }
                obj = arr;
            }
            else {
                for (name in obj) {
                    if (obj.hasOwnProperty(name)) {
                        if (typeof obj[name] === "object") {
                            item = obj[name];
                            if (item) {
                                id = item.$ref;
                                if (typeof id === "string") {
                                    obj[name] = catalog[id];
                                }
                                else {
                                    obj[name] = resolveReferences(item, catalog);
                                }
                            }
                        }
                    }
                }
            }
        }
        return obj;
    }
    function getDecycledCopy(obj, catalog) {
        // The createReferences function recurses through the object, producing the deep copy.
        var i; // The loop counter
        var name; // Property name
        var nu; // The new object or array
        switch (typeof obj) {
            case "object":
                // typeof null === 'object', so get out if this value is not really an object.
                // Also get out if it is a weird builtin object.
                if (obj === null || obj instanceof Boolean || obj instanceof Date || obj instanceof Number || obj instanceof RegExp || obj instanceof String) {
                    return obj;
                }
                for (i = 0; i < catalog.length; i += 1) {
                    if (catalog[i] === obj) {
                        return { $ref: i.toString() };
                    }
                }
                // Otherwise, accumulate the unique value and its id.
                obj.$id = catalog.length.toString();
                catalog.push(obj);
                // If it is an array, replicate the array.
                if (Object.prototype.toString.apply(obj) === "[object Array]") {
                    nu = [];
                    for (i = 0; i < obj.length; i += 1) {
                        nu[i] = getDecycledCopy(obj[i], catalog);
                    }
                }
                else {
                    // If it is an object, replicate the object.
                    nu = {};
                    for (name in obj) {
                        if (Object.prototype.hasOwnProperty.call(obj, name)) {
                            nu[name] = getDecycledCopy(obj[name], catalog);
                        }
                    }
                }
                return nu;
            case "number":
            case "string":
            case "boolean":
            default:
                return obj;
        }
    }
    /** Make a deep copy of an object or array, assuring that there is at most
        one instance of each object or array in the resulting structure. The
        duplicate references (which might be forming cycles) are replaced with
        an object of the form
          {$id: id}
        where the id is a simple string
    Modified from Douglas Crockford's cycle.js (https://github.com/douglascrockford/JSON-js/blob/master/cycle.js)
    Ported over to TypeScript, and modified to handle the reference schema that Json.NET uses.
*/
    function decycle(obj) {
        var catalog = []; // Keep a reference to each unique object or array
        var newObj = getDecycledCopy(obj, catalog);
        return newObj;
    }
    JsonNetDecycle.decycle = decycle;
    /** Restore an object that was reduced by decycle. Members whose values are objects of the form
         {$ref: id}
    are replaced with references to the value found by the id. This will
    restore cycles. The object will be mutated.

    So,
         var s = '{$id:1, members: [{$ref:"1"}]';
         return retrocycle(JSON.parse(s));
    produces an object containing an array which holds the object itself.
    */
    function retrocycle(obj) {
        var catalog = [];
        findReferences(obj, catalog);
        return resolveReferences(obj, catalog);
    }
    JsonNetDecycle.retrocycle = retrocycle;
})(JsonNetDecycle || (JsonNetDecycle = {}));