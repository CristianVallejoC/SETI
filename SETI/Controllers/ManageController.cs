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

        public List<int> GetProjectValid()
        {
            List<int> results = new List<int>();
            if (ModelState.IsValid)
            {
                List<int> query1 = new List<int>();
                List<int> query2 = new List<int>();
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

                results = query2.Except(query1).ToList();
            }
            return results;
        }

        public List<DataGeneral> GetResults(int broker, int broker2)
        {
            if (broker == 0 && broker == 0)
            {
                List<int> results = GetProjectValid();
            }

            List<DataGeneral> data = new List<DataGeneral>();
            List<int> temp = new List<int>();
            List<DataGeneral> tempData = new List<DataGeneral>();

            if (broker != 0)
            {
                if (broker2 != 0)
                {
                    tempData = _context.DataGeneral
                        .Where(x => x.BrokerId == broker || x.BrokerId == broker2)
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
                        .Where(x => x.BrokerId == broker)
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
            return data;
        }

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
            var listData = GetListData();
            ViewBag.List = new SelectList(listData, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Individual(int broker1Selection)
        {
            var results = GetResults(broker1Selection, 0);
            if (results.Count != 0)
            {
                decimal payback = CalculatePayback(results);
                decimal van = CalculateVAN(results);
                ViewBag.ResultVan = "El VAN calculado para el Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " es de: " + van;
                ViewBag.ResultPay = "El Payback calculado para el Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " es de: " + payback;
            }
            else
            {
                ViewBag.ResultVan = "Actualiza los registros antes de realizar esta operación";
                ViewBag.ResultPay = "";
            }
            var listData = GetListData();
            ViewBag.List = new SelectList(listData, "Value", "Text");

            return View();
        }

        public ActionResult Compare()
        {
            var listData = GetListData();

            ViewBag.List = new SelectList(listData, "Value", "Text");
            ViewBag.List2 = new SelectList(listData, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Compare(int broker1Selection, int broker2Selection)
        {
            var results = GetResults(broker1Selection, broker2Selection);
            decimal vanb1, vanb2, payb1, payb2;

            var temp1 = results.Where(b => b.BrokerId == broker1Selection).ToList();
            var temp2 = results.Where(b => b.BrokerId == broker2Selection).ToList();

            if (temp1.Count() != 0 && temp2.Count() != 0)
            {
                vanb1 = CalculateVAN(temp1);
                vanb2 = CalculateVAN(temp2);
                payb1 = CalculatePayback(temp1);
                payb2 = CalculatePayback(temp2);

                if (vanb1 > vanb2)
                {
                    ViewBag.ResultVan = "Los proyectos con la mejor opción en función del beneficio generado por la inversión los tiene el Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " (VAN)";
                }
                else
                {
                    ViewBag.ResultVan = "Los proyectos con la mejor opción en función del beneficio generado por la inversión los tiene el Broker: " + results.Where(dg => dg.BrokerId == broker2Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " (VAN)";
                }

                if (payb1 < payb2)
                {
                    ViewBag.ResultPay = "Los proyectos con la mejor opción en función de tiempo de recuperación los tiene el Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " (Payback)";
                }
                else
                {
                    ViewBag.ResultPay = "Los proyectos con la mejor opción en función de tiempo de recuperación los tiene el Broker: " + results.Where(dg => dg.BrokerId == broker2Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " (Payback)";
                }

                if (payb1 == payb2)
                {
                    ViewBag.ResultPay = "El Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " y el Broker: " + results.Where(dg => dg.BrokerId == broker2Selection).Select(dg => dg.BrokerName).FirstOrDefault() + "tienen la misma alternativa en función de tiempo de recuperación de la inversión (Payback)";
                }

                if (vanb1 == vanb2)
                {
                    ViewBag.ResultPay = "El Broker: " + results.Where(dg => dg.BrokerId == broker1Selection).Select(dg => dg.BrokerName).FirstOrDefault() + " y el Broker: " + results.Where(dg => dg.BrokerId == broker2Selection).Select(dg => dg.BrokerName).FirstOrDefault() + "tienen la misma alternativa en función de beneficio generado por la inversión (VAN)";
                }
            }
            else
            {
                ViewBag.ResultVan = "Actualiza los registros antes de realizar esta operación";
                ViewBag.ResultPay = "";
            }

            var listData = GetListData();
            ViewBag.List = new SelectList(listData, "Value", "Text");

            return View();
        }

        public ActionResult List()
        {
            var result = from data in _context.DataGeneral
                         group data by new { data.BrokerId, data.BrokerName } into brokerGroup
                         orderby brokerGroup.Average(d => d.ValuePayback) ascending,
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
            List<int> results = GetProjectValid().ToList();
            int batchSize = 100;
            int totalRecords = _context.DataGeneral.Count();

            for (int i = 0; i < totalRecords; i += batchSize)
            {
                var partition = _context.DataGeneral.OrderBy(p => p.ProjectId)
                                      .Skip(i)
                                      .Take(batchSize)
                                      .ToList();

                var recordsToDelete = partition.Where(p => !results.Contains(p.ProjectId)).ToList();

                _context.DataGeneral.RemoveRange(recordsToDelete);
                _context.SaveChanges();
            }
            return RedirectToAction("List", "Manage");
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