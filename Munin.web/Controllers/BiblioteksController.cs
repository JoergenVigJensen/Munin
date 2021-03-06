﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using Munin.DAL.Models;
using Munin.DAL;
using System.Threading.Tasks;
using Munin.web.ViewModels;
using Newtonsoft.Json;
using Bibliotek = Munin.DAL.Models.Bibliotek;

namespace Munin.web.Controllers
{
    public class BiblioteksController : Controller
    {
        // GET: Biblioteks
        public ActionResult Index()
        {
            using (var db = new ILABNew2Entities())
            {
                var bibliotek = db.Bibliotek.Include(b => b.Journaler);
                return View(bibliotek.ToList());
            }
        }


        public async Task<ActionResult> BogListe(ListQuery query)
        {
            try
            {
                using (var dbmunin = new ILABNew2Entities())
                {

                    int flicks = query.P * query.Size;

                    var l = await dbmunin.Bibliotek.ToListAsync();

                    if (!string.IsNullOrEmpty(query.Q))
                    {
                        string[] sQuery = query.Q.Split(' ');
                        for (int i = 0; i < sQuery.Length; i++)
                        {
                            l =
                                l.Where(
                                    x => (x.Bogkode != null && x.Bogkode.ToLower().Contains(sQuery[i].ToLower())) ||
                                         (x.Note != null && x.Note.ToLower().Contains(sQuery[i].ToLower())) ||
                                         (x.Titel != null && x.Titel.ToLower().Contains(sQuery[i].ToLower())) ||
                                         (x.Forfatter != null && x.Forfatter.ToLower().Contains(sQuery[i].ToLower())) ||
                                         (x.Undertitel != null && x.Undertitel.ToLower().Contains(sQuery[i].ToLower())))
                                    .ToList();
                        }
                    }

                    var column = typeof(Bibliotek).GetProperty(query.S,
                        BindingFlags.SetProperty | BindingFlags.IgnoreCase |
                        BindingFlags.Public | BindingFlags.Instance);


                    if (column != null)
                    {
                        if (query.O.ToUpper() == "DESC")
                        {
                            l = l.OrderByDescending(x => column.GetValue(x, null)).ToList();
                        }
                        else
                        {
                            l = l.OrderBy(x => column.GetValue(x, null)).ToList();
                        }
                    }

                    var pageResult = l.Select(x => new BiblioteksDto()
                    {
                        ID = x.BibliotekID,
                        BogKode = x.Bogkode,
                        Titel = x.Titel,
                        Forfatter = x.Forfatter,
                        Udgivet = (x.Udgivet != null)? x.Udgivet.Value:0,
                    }).Skip(1).Take(query.Size);

                    if (l.Count > flicks)
                    {
                        pageResult = l.Select(x => new BiblioteksDto()
                        {
                            ID = x.BibliotekID,
                            BogKode = x.Bogkode,
                            Titel = x.Titel,
                            Forfatter = x.Forfatter,
                            Udgivet = (x.Udgivet != null) ? x.Udgivet.Value : 0,

                        }).Skip(flicks).Take(query.Size);
                    }

                    var listResult = new MuninListViewModel<BiblioteksDto>()
                    {
                        Count = l.Count,
                        Pages = l.Count / query.Size,
                        Data = pageResult.ToList()
                    };

                    var result = JsonConvert.SerializeObject(listResult, Utils.JsonSettings());

                    return Content(result);
                }
            }
            catch (Exception ex)
            {
                return Content("Fejl:" + ex.Message);
            }
        }

        public async Task<ActionResult> Load(int id = 0)
        {
            var vm = new BibliotekVm();

            try
            {
                using (ILABNew2Entities db = new ILABNew2Entities())
                {
                    vm.JournalList =
                        await db.Journaler.Select(x => new UISelectItem() {Value = x.JournalID, Text = x.JournalNb})
                            .ToListAsync();

                    vm.Model = new Bibliotek();


                    if (id > 0)
                    {
                        var bibliotek = db.Bibliotek.FirstOrDefault(x => x.BibliotekID == id);
                        vm.Model = new Bibliotek()
                        {
                            JournalID = 0,
                        };

                        if (bibliotek != null)
                        {
                            vm.Model = new Bibliotek()
                            {
                                BibliotekID = bibliotek.BibliotekID,
                                BogID = bibliotek.BibliotekID,
                                Bogkode = bibliotek.Bogkode,
                                Titel = bibliotek.Titel,
                                Forfatter = bibliotek.Forfatter,
                                Udgivet = (bibliotek.Udgivet == null) ? 0 : bibliotek.Udgivet.Value,
                                DK5 = bibliotek.DK5,
                                Forlag = bibliotek.Forlag,
                                Note = bibliotek.Note,
                                Indlevering = bibliotek.Indlevering,
                                Journal = bibliotek.Journal,
                                JournalID = (bibliotek.JournalID == null) ? 0 : bibliotek.JournalID.Value,
                                Ordningsord = bibliotek.Ordningsord,
                                Redaktor = bibliotek.Redaktor,
                                Samlemappe = bibliotek.Samlemappe,
                                Undertitel = bibliotek.Undertitel
                            };
                        }
                    }
                    else
                    {
                        var bogkode = db.Bibliotek.OrderByDescending(x => x.Bogkode).First().Bogkode;
                        int bindex = Int32.Parse(bogkode.Split('.')[1]);
                        bindex++;
                        vm.Model.Bogkode = "BB." + bindex.ToString().PadLeft(4, '0');
                    }
                }

                var result = JsonConvert.SerializeObject(vm, Utils.JsonSettings());
                return Content(result);
            }
            catch (Exception ex)
            {
                var result = JsonConvert.SerializeObject(ex.Message, Utils.JsonSettings());
                return Content(result);

            }
        }


        // GET: Biblioteks/Details/5
        public ActionResult Details(int? id)
        {
            using (var db = new ILABNew2Entities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Bibliotek bibliotek = db.Bibliotek.Find(id);
                if (bibliotek == null)
                {
                    return HttpNotFound();
                }
                return View(bibliotek);
            }
        }


        [HttpPost]
        public async Task<ActionResult> Save(Bibliotek model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = ModelState.Keys.SelectMany(k => ModelState[k].Errors).First().ErrorMessage
                });

            try
            {
                using (var db = new ILABNew2Entities())
                {

                    var dbModel = new Bibliotek();

                    if (model.BibliotekID > 0)
                    {
                        dbModel = await db.Bibliotek.FirstOrDefaultAsync(x => x.BibliotekID == model.BibliotekID);
                        if (dbModel == null)
                            throw new Exception(string.Format("Bog med id {0} blev ikke fundet",model.BibliotekID));
                    }

                    dbModel.Bogkode = model.Bogkode;
                    dbModel.Titel = model.Titel;
                    dbModel.Udgivet = (model.Udgivet != null) ? model.Udgivet.Value : 0;
                    dbModel.Forfatter = model.Forfatter;
                    dbModel.Forlag = model.Forlag;
                    dbModel.Undertitel = model.Undertitel;
                    dbModel.DK5 = model.DK5;
                    dbModel.Ordningsord = model.Ordningsord;
                    dbModel.Redaktor = model.Redaktor;
                    dbModel.Journal = model.Journal;
                    dbModel.JournalID = model.JournalID;
                    dbModel.Samlemappe = model.Samlemappe;
                    dbModel.Note = model.Note;

                    if (model.BibliotekID > 0)
                        db.Entry(dbModel).State = EntityState.Modified;
                    else
                    {
                        db.Bibliotek.Add(dbModel);
                    }

                    await db.SaveChangesAsync();
                    return Json(new { success = true, message = "" });
                }
            }
            catch (Exception ex)
            {
                //brug extensible funktion
                string err_message = ex.Message;

                if (ex.InnerException != null)
                {
                    err_message = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                        err_message = ex.InnerException.InnerException.Message;
                }

                return Json(new
                {
                    success = false,
                    message = err_message
                });
            }

        }

        // GET: Biblioteks/Create
        public ActionResult Create()
        {
            var vm = new BibliotekVm();
            vm.Model = new Bibliotek();
            vm.Model.BibliotekID = 0;

            return View();
        }
    
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new ILABNew2Entities())
            {
                Bibliotek bibliotek = db.Bibliotek.Find(id);
                if (bibliotek == null)
                {
                    return HttpNotFound();
                }
                return View(bibliotek);
            }
        }

      // POST: Biblioteks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new ILABNew2Entities())
            {
                Bibliotek bibliotek = db.Bibliotek.Find(id);
                db.Bibliotek.Remove(bibliotek);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                using (var db = new ILABNew2Entities())
                {
                    db.Dispose();
                }                    
            }
            base.Dispose(disposing);
        }
    }
}
