using Microsoft.AspNetCore.Mvc;
using Calculator.Business;
using Calculator.CommonLayer.Dto;
using System.Globalization;
using Calculator.CommonLayer.Dto;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;

namespace Calculator.Web.Controllers
{
    public class CalculatorController : Controller
    {
        private readonly CalculatorService _calculatorService;

        public CalculatorController()
        {
            _calculatorService = new CalculatorService();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AddDto());  // AddDto'yu view'a gönderir
        }

        [HttpPost]
        public IActionResult Index(string expression)
        {
            var model = new AddDto();

            if (string.IsNullOrEmpty(expression))
            {
                ModelState.AddModelError("", "İfade boş olamaz.");
                return View(model);
            }

            try
            {
                var result = EvaluateExpression(expression);
                model.Result = result;
                ViewBag.Result = result;

                // İşlem geçmişine ekleme
                var history = HttpContext.Session.GetObjectFromJson<List<CalculationHistory>>("CalculationHistory") ?? new List<CalculationHistory>();
                var parsedExpression = ParseExpression(expression);
                history.Add(new CalculationHistory
                {
                    Number1 = parsedExpression.Number1,
                    Number2 = parsedExpression.Number2,
                    Operation = parsedExpression.Operation,
                    Result = result,
                    Expression = expression,
                    Timestamp = DateTime.Now
                });
                HttpContext.Session.SetObjectAsJson("CalculationHistory", history);  // Geçmiş verileri JSON formatında kaydeder
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

            return View(model);
        }

        private double EvaluateExpression(string expression)
        {
            // Basit bir ifade değerlendirme fonksiyonu, daha gelişmişi için farklı yöntemler kullanılabilir.
            var table = new DataTable();
            var value = table.Compute(expression, string.Empty);
            return Convert.ToDouble(value);
        }

        private (double Number1, double Number2, string Operation) ParseExpression(string expression)
        {
            // Basit bir ifade parse etme fonksiyonu, daha gelişmişi için farklı yöntemler kullanılabilir.
            string[] operators = { "+", "-", "*", "/" };
            foreach (var op in operators)
            {
                var parts = expression.Split(new[] { op }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    return (Convert.ToDouble(parts[0]), Convert.ToDouble(parts[1]), op);
                }
            }
            throw new FormatException("Geçersiz ifade formatı.");
        }
    }
}
