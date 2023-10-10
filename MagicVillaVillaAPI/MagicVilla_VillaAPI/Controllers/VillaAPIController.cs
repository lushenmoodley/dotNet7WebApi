﻿using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{

    [Route("api/VillaAPI")]
    [ApiController]//this tells the application this will be an api controller
    public class VillaAPIController:Controller
    {
        private readonly ApplicationDBContext _db;
        
        public VillaAPIController(ApplicationDBContext db)
        {
            _db = db;
        }


        //creating an end point
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult <IEnumerable<VillaDTO>> GetVillas()
        {
           return Ok(_db.Villas.ToList());
        }

        [HttpGet("{id:int}",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<VillaDTO> GetVilla(int id)//you have to pass an id in the http get or else it would crash
        {
            if(id==0)
            {
            
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

            if(villa==null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            /*if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            */

            if(_db.Villas.FirstOrDefault(x=>x.Name.ToLower()==villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Custom Error","Villa Already Exist!");
                return BadRequest(ModelState);
            }


            if(villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            if(villaDTO.Id>0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
            };


            _db.Villas.Add(model);
            _db.SaveChanges();
            
            return CreatedAtRoute("GetVilla",new { id=villaDTO.Id}, villaDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

            if (villa==null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();

        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if(villaDTO==null || id!=villaDTO.Id)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);

            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy= villaDTO.Occupancy;

            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
            };
            _db.Villas.Update(model);
            _db.SaveChanges();

            return NoContent();

        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument <VillaDTO> villaDTO)
        {
            if(villaDTO==null || id==0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.AsNoTracking().FirstOrDefault(x => x.Id == id);

            
            Villa villaDTO2 = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };

            Villa model = new Villa()
            {
                Amenity = villaDTO2.Amenity,
                Details = villaDTO2.Details,
                Id = villaDTO2.Id,
                ImageUrl = villaDTO2.ImageUrl,
                Name = villaDTO2.Name,
                Occupancy = villaDTO2.Occupancy,
                Rate = villaDTO2.Rate,
                Sqft = villaDTO2.Sqft,
            };


            if (villa==null)
            {
                return BadRequest();
            }
            /*
            _db.Villas.Update(model);
            _db.SaveChanges();*/

           if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();

        }
    }
}
