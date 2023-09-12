using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cell_line_laboratory.Data;
using Cell_line_laboratory.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Cell_line_laboratory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly Cell_line_laboratoryContext _context;

        public DataController(Cell_line_laboratoryContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //[Route("GetCounts")]
        //public IActionResult GetCounts()
        //{
        //    var dto = new CellLineCountDto();
        //    dto.GetCellLineCounts(_context, "Used");
        //    dto.GetCellLineCounts(_context, "Semi-Used");
        //    dto.GetCellLineCounts(_context, "Unused");

        //    return Ok(dto);
        //}

        [HttpGet]
        [Route("GetCounts")]
        public IActionResult GetCounts(int year, int month)  // Add parameters for year and month
        {
            var cellLineDto = new CellLineCountDto();
            var enzymeDto = new EnzymeCountDto();
            var chemicalDto = new ChemicalCountDto();
            var plasmidDto = new PlasmidCountDto();

            cellLineDto.GetCellLineCounts(_context, "Used", year, month);  // Pass year and month
            cellLineDto.GetCellLineCounts(_context, "Semi-Used", year, month);  // Pass year and month
            cellLineDto.GetCellLineCounts(_context, "Unused", year, month);  // Pass year and month

            enzymeDto.GetEnzymeCounts(_context, "Used", year, month);  // Pass year and month
            enzymeDto.GetEnzymeCounts(_context, "Semi-Used", year, month);  // Pass year and month
            enzymeDto.GetEnzymeCounts(_context, "Unused", year, month);  // Pass year and month

            chemicalDto.GetChemicalCounts(_context, "Used", year, month);  // Pass year and month
            chemicalDto.GetChemicalCounts(_context, "Semi-Used", year, month);  // Pass year and month
            chemicalDto.GetChemicalCounts(_context, "Unused", year, month);  // Pass year and month

            plasmidDto.GetPlasmidCounts(_context, "Used", year, month);  // Pass year and month
            plasmidDto.GetPlasmidCounts(_context, "Semi-Used", year, month);  // Pass year and month
            plasmidDto.GetPlasmidCounts(_context, "Unused", year, month);  // Pass year and month

            var result = new
            {
                cellLine = new { cellLineDto.UsedCount, cellLineDto.SemiUsedCount, cellLineDto.UnusedCount },
                enzymes = new { enzymeDto.UsedCount, enzymeDto.SemiUsedCount, enzymeDto.UnusedCount },
                chemicals = new { chemicalDto.UsedCount, chemicalDto.SemiUsedCount, chemicalDto.UnusedCount },
                plasmids = new { plasmidDto.UsedCount, plasmidDto.SemiUsedCount, plasmidDto.UnusedCount }
            };

            return Ok(result);
        }


    }
}