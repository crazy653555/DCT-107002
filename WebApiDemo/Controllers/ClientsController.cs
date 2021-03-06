﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
{
    public class ClientsController : ApiController
    {
        private FabricsEntities db = new FabricsEntities();

        public ClientsController()
        {
            db.Configuration.LazyLoadingEnabled = false;
        }

        [Route("clients")]
        public IQueryable<Client> GetClient()
        {
            return db.Client.OrderByDescending(p => p.ClientId).Take(10);
        }

        [Route("clients/{id:int}")]
        [ResponseType(typeof(Client))]
        public HttpResponseMessage GetClient(int id)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //return Request.CreateResponse(HttpStatusCode.OK, client);
            return new HttpResponseMessage()
            {
                ReasonPhrase = "I CAN",
                Content = new ObjectContent<Client>(client,
                  GlobalConfiguration.Configuration.Formatters.JsonFormatter)
            };
        }



        [Route("clients/{*date}")]
        [ResponseType(typeof(Client))]
        public IHttpActionResult GetClientByBirth(DateTime date)
        {
            Client client = db.Client.FirstOrDefault(p => p.DateOfBirth == date);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        [Route("FromBodyTest")]
        public IHttpActionResult PostA([FromBody]string name)
        {
            return Ok(name);
        }

        [Route("FromUriTest")]
        public IHttpActionResult GetA([FromUri]GeoPoint geo)
        {
            return Ok(geo);
        }


        // PUT: api/Clients/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClient(int id, Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != client.ClientId)
            {
                return BadRequest();
            }

            db.Entry(client).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Clients
        [ResponseType(typeof(Client))]
        public IHttpActionResult PostClient(Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Client.Add(client);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = client.ClientId }, client);
        }

        // DELETE: api/Clients/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult DeleteClient(int id)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            db.Client.Remove(client);
            db.SaveChanges();

            return Ok(client);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientExists(int id)
        {
            return db.Client.Count(e => e.ClientId == id) > 0;
        }
    }
}