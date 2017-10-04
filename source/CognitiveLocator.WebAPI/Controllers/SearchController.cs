﻿using CognitiveLocator.WebAPI.Class;
using CognitiveLocator.WebAPI.Models;
using CognitiveLocator.WebAPI.Models.FaceApiModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CognitiveLocator.WebAPI.Controllers
{
    [RoutePrefix("api/Find")]
    public class SearchController : ApiController
    {
        private SPQuery querySp = new SPQuery();

        [Route("ByFace")]
        [HttpPost]
        [System.Web.Mvc.RequireHttps]
        public async Task<IHttpActionResult> ByFace()
        {
            try
            {
                Guid OperationID = Guid.NewGuid();
                byte[] fileBytes = null;
                String FileName = string.Empty;
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                string root = "D:/local/Temp/"; //HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                try
                {
                    var filesReadToProvider = await Request.Content.ReadAsMultipartAsync(provider);
                    fileBytes = File.ReadAllBytes(provider.FileData.First().LocalFileName);
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        FileName = file.Headers.ContentDisposition.FileName;
                    }

                    ClassStg storage = new ClassStg();
                    Person ObjPerson = new Person();

                    string uri = await storage.UploadPhoto(new MemoryStream(fileBytes), FileName);
                    FaceAPIMethods ObjFaceApiPerson = new FaceAPIMethods(OperationID);
                    List<JObject> detectResult = await ObjFaceApiPerson.DetectFace(uri);
                    string detectFaceId = detectResult.First()["faceId"].ToString();
                    List<FindSimilar> similarFace = await ObjFaceApiPerson.FindSimilarFace(detectFaceId);
                    File.Delete(provider.FileData.First().LocalFileName);
                    List<Person> listPFaceIdComplete = new List<Person>();

                    foreach (var i in similarFace)
                    {
                        List<Person> listPFaceId = new List<Person>();
                        listPFaceId = await querySp.SelectPersonByFaceId(i.persistedFaceId);
                        if (listPFaceId.Count()!= 0) {
                            ObjPerson = listPFaceId.First();
                            listPFaceIdComplete.Add(ObjPerson);
                        }
                    }


                    return Ok(listPFaceIdComplete.First());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.Message);
                    return InternalServerError();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                throw;
            }
        }

        [Route("ByName")]
        [HttpGet]
        [System.Web.Mvc.RequireHttps]
        public async Task<IHttpActionResult> ByName([FromUri] string name)
        {
            try
            {
                List<Person> listByName = new List<Person>();
                listByName = await querySp.SelectPersonByName(name);
                return Ok(listByName);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
                throw;
            }

        }

        [Route("ByLastName")]
        [HttpGet]
        [System.Web.Mvc.RequireHttps]
        public async Task<IHttpActionResult> ByLastName([FromUri] string lastName)
        {
            try
            {
                List<Person> listByLastName = new List<Person>();
                listByLastName = await querySp.SelectPersonByLastName(lastName);
                return Ok(listByLastName);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
                throw;
            }
        }

        [Route("UpdataFoundPerson")]
        [HttpGet]
        [System.Web.Mvc.RequireHttps]
        public async Task<IHttpActionResult> UpdateFoundPerson([FromUri] string idPerson, int isFound, string location ="")
        {
            try
            {
                IEnumerable<dynamic> listUpdate = null;
                listUpdate = await querySp.UpdateFoundPerson(idPerson, isFound, location);
                return Ok(listUpdate);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
                throw;
            }
        }
        //[Route("PersonDisable")]
        //public async Task<IHttpActionResult> DisablePerson([FromUri] string idPerson)
        //{
        //    try
        //    {
        //        IEnumerable<dynamic> listDisableP = null;
        //        listDisableP = await querySp.DisablePerson(idPerson);
        //        return Ok(listDisableP);
        //    }
        //    catch (Exception e)
        //    {
        //        return InternalServerError(e);
        //        throw;
        //    }
        //}

        [Route("PersonEnable")]
        [HttpGet]
        [System.Web.Mvc.RequireHttps]
        public async Task<IHttpActionResult> EnablePerson([FromUri] string idPerson)
        {
            try
            {
                IEnumerable<dynamic> listEnableP = null;
                listEnableP = await querySp.EnablePerson(idPerson);
                return Ok(listEnableP);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
                throw;
            }
        }

        [Route("ByNameAndLastName")]
        [HttpGet]
        [System.Web.Mvc.RequireHttps]
        public async Task<IHttpActionResult> SelectPersonByNameAndLastName([FromUri] string name, string lastName)
        {
            try
            {
                List<Person> listPerson = new List<Person>();
                listPerson = await querySp.SelectPersonByNameAndLastName(name,lastName);
                return Ok(listPerson);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
                throw;
            }
        }
    }
}
