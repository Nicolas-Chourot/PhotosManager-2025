using PhotosManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using static PhotosManager.Controllers.AccessControl;

namespace PhotosManager.Controllers
{
    [UserAccess]
    public class PhotosController : Controller
    {
        const string IllegalAccessUrl = "/Accounts/Login?message=Tentative d'accès illégal!&success=false";

        public ActionResult SetPhotoOwnerSearchId(int id)
        {
            Session["photoOwnerSearchId"] = id;
            return RedirectToAction("List");
        }
        public ActionResult SetSearchKeywords(string keywords)
        {
            Session["searchKeywords"] = keywords;
            return RedirectToAction("List");
        }
        public ActionResult GetPhotos(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Photos.HasChanged || DB.Likes.HasChanged || DB.Users.HasChanged)
            {               
                return PartialView(DB.Photos.ToList().OrderByDescending(p => p.CreationDate).ToList());
            }
            return null;
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View(new Photo());
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Photo photo)
        {
            photo.OwnerId = ((User)Session["ConnectedUser"]).Id;
            photo.CreationDate = DateTime.Now;
            DB.Photos.Add(photo);
            return RedirectToAction("List");
        }
        public ActionResult Edit()
        {
            if (Session["id"] != null)
            {
                int id = (int)Session["id"];
                Photo photo = DB.Photos.Get(id);
                User connectedUser = (User)Session["ConnectedUser"];
                if (photo != null)
                {
                    if (connectedUser.IsAdmin || photo.OwnerId == connectedUser.Id)
                    {
                        return View(photo);
                    }
                    return Redirect(IllegalAccessUrl);
                }
            }
            return Redirect(IllegalAccessUrl);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(Photo photo)
        {
            User connectedUser = ((User)Session["ConnectedUser"]);
            if (Session["IsOwner"] != null? (bool)Session["IsOwner"] : false)
            {
                Photo storedPhoto = DB.Photos.Get((int)Session["id"]);
                photo.Id = storedPhoto.Id;
                photo.OwnerId = storedPhoto.OwnerId;
                photo.CreationDate = storedPhoto.CreationDate;
                DB.Photos.Update(photo);
                return RedirectToAction("List");
            }
            return Redirect(IllegalAccessUrl);
        }
        public ActionResult Details(int id)
        {
            Photo photo = DB.Photos.Get(id);
            if (photo != null)
            {
                Session["id"] = id;
                User connectedUser = ((User)Session["ConnectedUser"]);
                Session["IsOwner"] = connectedUser.IsAdmin || photo.OwnerId == connectedUser.Id;    
                if ((bool)Session["IsOwner"] || photo.Shared)
                    return View(photo);
                else
                    return Redirect(IllegalAccessUrl);
            }
            return Redirect(IllegalAccessUrl);
        }
        public ActionResult Delete()
        {
            if (Session["id"] != null)
            {
                int id = (int)Session["id"];
                Photo photo = DB.Photos.Get(id);
                if (photo != null)
                {
                    User connectedUser = (User)Session["ConnectedUser"];
                    if (connectedUser.IsAdmin || photo.OwnerId == connectedUser.Id)
                    {
                        DB.Photos.Delete(id);
                        return RedirectToAction("List");
                    }
                    else
                        return Redirect(IllegalAccessUrl);
                }
            }
            return Redirect(IllegalAccessUrl);
        }
        public ActionResult TogglePhotoLike(int id)
        {
            User connectedUser = (User)Session["ConnectedUser"];
            DB.Likes.ToggleLike(id, connectedUser.Id);
            return RedirectToAction("Details/" + id);
        }
    }
}