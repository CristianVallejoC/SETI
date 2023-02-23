using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using SETI.Areas.Identity.Data;
using SETI.Models;
using System.Data;

namespace SETI.Controllers
{
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        // GET: ManageController
        public ManageController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public ActionResult Index()
        {
            return View();
        }

        public class DataList
        {
            public int BrokerId { get; set; }
            public string BrokerName { get; set; }
            public decimal ValuePayback { get; set; }
            public decimal ValueVan { get; set; }
        }

        //Me trae todos los datos que cumplen con el requerimiento y sus indicaciones (Sirve para actualizar tabla)
        public List<DataGeneral> GetResults(int broker, int broker2)
        {
            List<DataGeneral> data = new List<DataGeneral>();

            if (_context.DataGeneral.Count() != 0)
            {
                if (ModelState.IsValid)
                {
                    List<int> query1 = new List<int>();
                    List<int> query2 = new List<int>();

                    if (broker != 0)
                    {
                        if (broker2 != 0)
                        {
                            query1 = (from b in _context.Broker
                                      join ip in _context.InvestmentProject on b.BrokerId equals ip.BrokerId
                                      join pm in _context.ProjectMovement on ip.ProjectId equals pm.ProjectId
                                      join p in _context.Period on pm.PeriodId equals p.PeriodId
                                      where p.PeriodYear < 2023 && b.BrokerId == broker || b.BrokerId == broker2
                                      group ip by ip.ProjectId into g
                                      where g.Count() >= 1
                                      select g.Key).ToList();

                            query2 = (from b in _context.Broker
                                      join ip in _context.InvestmentProject on b.BrokerId equals ip.BrokerId
                                      join pm in _context.ProjectMovement on ip.ProjectId equals pm.ProjectId
                                      join p in _context.Period on pm.PeriodId equals p.PeriodId
                                      where p.PeriodYear >= 2023 && b.BrokerId == broker || b.BrokerId == broker2
                                      group ip by ip.ProjectId into g
                                      where g.Count() >= 1
                                      select g.Key).ToList();
                        }
                        else
                        {
                            query1 = (from b in _context.Broker
                                      join ip in _context.InvestmentProject on b.BrokerId equals ip.BrokerId
                                      join pm in _context.ProjectMovement on ip.ProjectId equals pm.ProjectId
                                      join p in _context.Period on pm.PeriodId equals p.PeriodId
                                      where p.PeriodYear < 2023 && b.BrokerId == broker
                                      group ip by ip.ProjectId into g
                                      where g.Count() >= 1
                                      select g.Key).ToList();

                            query2 = (from b in _context.Broker
                                      join ip in _context.InvestmentProject on b.BrokerId equals ip.BrokerId
                                      join pm in _context.ProjectMovement on ip.ProjectId equals pm.ProjectId
                                      join p in _context.Period on pm.PeriodId equals p.PeriodId
                                      where p.PeriodYear >= 2023 && b.BrokerId == broker
                                      group ip by ip.ProjectId into g
                                      where g.Count() >= 1
                                      select g.Key).ToList();
                        }
                    }
                    else
                    {
                        query1 = (from b in _context.Broker
                                  join ip in _context.InvestmentProject on b.BrokerId equals ip.BrokerId
                                  join pm in _context.ProjectMovement on ip.ProjectId equals pm.ProjectId
                                  join p in _context.Period on pm.PeriodId equals p.PeriodId
                                  where p.PeriodYear < 2023
                                  group ip by ip.ProjectId into g
                                  where g.Count() >= 1
                                  select g.Key).ToList();

                        query2 = (from b in _context.Broker
                                  join ip in _context.InvestmentProject on b.BrokerId equals ip.BrokerId
                                  join pm in _context.ProjectMovement on ip.ProjectId equals pm.ProjectId
                                  join p in _context.Period on pm.PeriodId equals p.PeriodId
                                  where p.PeriodYear >= 2023
                                  group ip by ip.ProjectId into g
                                  where g.Count() >= 1
                                  select g.Key).ToList();
                    }

                    List<int> results = query2.Except(query1).ToList();


                    List<int> temp = new List<int>();
                    List<DataGeneral> tempData = new List<DataGeneral>();

                    if (broker != 0)
                    {
                        if (broker2 != 0)
                        {
                            tempData = _context.DataGeneral
                                .Where(x => results.Contains(x.ProjectId) && x.BrokerId == broker || x.BrokerId == broker2)
                                .Select(x => new DataGeneral
                                {
                                    BrokerId = x.BrokerId,
                                    BrokerName = x.BrokerName,
                                    ProjectId = x.ProjectId,
                                    InvestmentAmount = x.InvestmentAmount,
                                    ValueVan = x.ValueVan,
                                    ValuePayback = x.ValuePayback
                                }).ToList();
                        }
                        else
                        {
                            tempData = _context.DataGeneral
                                .Where(x => results.Contains(x.ProjectId) && x.BrokerId == broker)
                                .Select(x => new DataGeneral
                                {
                                    BrokerId = x.BrokerId,
                                    BrokerName = x.BrokerName,
                                    ProjectId = x.ProjectId,
                                    InvestmentAmount = x.InvestmentAmount,
                                    ValueVan = x.ValueVan,
                                    ValuePayback = x.ValuePayback
                                }).ToList();
                        }
                    }
                    else
                    {
                        tempData = _context.DataGeneral
                         .Where(x => results.Contains(x.ProjectId))
                         .Select(x => new DataGeneral
                         {
                             BrokerId = x.BrokerId,
                             BrokerName = x.BrokerName,
                             ProjectId = x.ProjectId,
                             InvestmentAmount = x.InvestmentAmount,
                             ValueVan = x.ValueVan,
                             ValuePayback = x.ValuePayback
                         }).ToList();
                    }

                    data.AddRange(tempData);
                    data = data.OrderBy(ob => ob.BrokerId).ToList();
                }
            }

            return data;
        }

        //Permite traer lista de Brokers con la finalidad de iterar sobre ellos
        public List<SelectListItem> GetListData()
        {
            var data = _context.Broker
                .ToList();

            var selectList = data.Select(b => new SelectListItem
            {
                Value = b.BrokerId.ToString(),
                Text = b.BrokerName
            }).ToList();

            return selectList;
        }

        public ActionResult Individual()
        {
            // Retrieve data for the first dropdown list
            var listData = GetListData();

            // Populate the dropdown lists with the retrieved data
            ViewBag.List = new SelectList(listData, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Individual(int broker1Selection)
        {
            // Obtener los resultados de la comparación
            var results = GetResults(broker1Selection, 0);

            // Calcular VAN y payback
            decimal payback = CalculatePayback(results);
            decimal van = CalculateVAN(results);
            ViewBag.ResultVan = "El VAN calculado para el Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " es de: " + van;
            ViewBag.ResultPay = "El Payback calculado para el Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " es de: " + payback;

            var listData = GetListData();
            ViewBag.List = new SelectList(listData, "Value", "Text");

            return View();
        }

        public ActionResult Compare()
        {
            // Retrieve data for the first dropdown list
            var listData = GetListData();

            // Populate the dropdown lists with the retrieved data
            ViewBag.List = new SelectList(listData, "Value", "Text");
            ViewBag.List2 = new SelectList(listData, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Compare(int broker1Selection, int broker2Selection)
        {
            //// Obtener los resultados de la comparación
            var results = GetResults(broker1Selection, broker2Selection);
            decimal vanb1, vanb2, payb1, payb2;

            var temp1 = results.Where(b => b.BrokerId == broker1Selection).ToList();
            var temp2 = results.Where(b => b.BrokerId == broker2Selection).ToList();

            vanb1 = CalculateVAN(temp1);
            vanb2 = CalculateVAN(temp2);
            payb1 = CalculatePayback(temp1);
            payb2 = CalculatePayback(temp2);

            // Comparar los promedios y establecer los resultados de la vista
            if (vanb1 > vanb2)
            {
                ViewBag.ResultVan = "Los proyectos con la mejor opción en función del beneficio generado por la inversión los tiene el Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " (VAN)";
            }
            else
            {
                ViewBag.ResultVan = "Los proyectos con la mejor opción en función del beneficio generado por la inversión los tiene el Broker: " + results.Where(dg => dg.BrokerId == broker2Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " (VAN)";
            }

            if (payb1 > payb2)
            {
                ViewBag.ResultPay = "Los proyectos con la mejor opción en función de tiempo de recuperación los tiene el Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " (Payback)";
            }
            else
            {
                ViewBag.ResultPay = "Los proyectos con la mejor opción en función de tiempo de recuperación los tiene el Broker: " + results.Where(dg => dg.BrokerId == broker2Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " (Payback)";
            }

            var listData = GetListData();
            ViewBag.List = new SelectList(listData, "Value", "Text");

            return View();
        }

        public ActionResult List()
        {
            var result = from data in _context.DataGeneral
                         group data by new { data.BrokerId, data.BrokerName } into brokerGroup
                         orderby brokerGroup.Average(d => d.ValuePayback) descending,
                                 brokerGroup.Average(d => d.ValueVan) descending
                         select new DataList
                         {
                             BrokerId = brokerGroup.Key.BrokerId,
                             BrokerName = brokerGroup.Key.BrokerName,
                             ValueVan = brokerGroup.Average(d => d.ValueVan),
                             ValuePayback = brokerGroup.Average(d => d.ValuePayback)
                         };

            return View(result.ToList());
        }

        public decimal CalculateVAN(List<DataGeneral> list)
        {
            var brokerVAN = list.Average(d => d.ValueVan);
            return brokerVAN;
        }

        public decimal CalculatePayback(List<DataGeneral> list)
        {
            var brokerPayback = list.Average(d => d.ValuePayback);
            return brokerPayback;
        }

        public async Task<IActionResult> UpdateAsync()
        {
            await UpdateProcedureAsync();

            var results = GetResults(0, 0).Select(dg => dg.ProjectId).ToList();
            var dataToRemove = _context.DataGeneral.Where(dg => !results.Contains(dg.ProjectId));
            _context.DataGeneral.RemoveRange(dataToRemove);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> UpdateProcedureAsync()
        {
            try
            {
                await ExecuteProcedureAsync("devtest.sp_updateBalances");
            }
            catch (Exception)
            {
                ModelState.AddModelError("Procedure", "Error al ejecutar el procedimiento almacenado");
                throw;
            }

            return Ok();
        }

        public async Task<int> ExecuteProcedureAsync(string procedureName, SqlParameter[] parameters = null, int commandTimeout = 300)
        {
            int rowsAffected = 0;
            string conect = _configuration.GetConnectionString("ApplicationDbContextConnection");

            using (SqlConnection connection = new SqlConnection(conect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = commandTimeout;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    if (_context.Database.GetDbConnection().State != ConnectionState.Open)
                    {
                        await _context.Database.GetDbConnection().OpenAsync();
                    }
                    rowsAffected = await command.ExecuteNonQueryAsync();
                    if (_context.Database.GetDbConnection().State != ConnectionState.Closed)
                    {
                        _context.Database.GetDbConnection().Close();
                    }
                }
            }
            return rowsAffected;
        }
    }
}