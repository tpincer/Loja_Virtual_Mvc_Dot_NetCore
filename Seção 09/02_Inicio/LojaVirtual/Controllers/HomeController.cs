﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LojaVirtual.Libraries.Email;
using LojaVirtual.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;
using LojaVirtual.Database;

namespace LojaVirtual.Controllers {
    public class HomeController : Controller {
        private LojaVirtualContext _banco;
        public HomeController(LojaVirtualContext banco)
        {
            _banco = banco;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index([FromForm] NewsletterEmail newsletter)
        {
            if (ModelState.IsValid)
            {
                _banco.NewsletterEmails.Add(newsletter);
                _banco.SaveChanges();

                TempData["MSG_S"] = "E-mail cadastrado! Agora você vai receber promoções especiais no seu e-mail! Fique atento as novidades!";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }
        }

        public IActionResult Contato()
        {
            return View();
        }
        public IActionResult ContatoAcao()
        {
            try
            {
                Contato contato = new Contato();
                contato.Nome = HttpContext.Request.Form["nome"];
                contato.Email = HttpContext.Request.Form["email"];
                contato.Texto = HttpContext.Request.Form["texto"];

                var listaMensagens = new List<ValidationResult>();
                var contexto = new ValidationContext(contato);
                bool isValid = Validator.TryValidateObject(contato, contexto, listaMensagens, true);

                if (isValid)
                {
                    ContatoEmail.EnviarContatoPorEmail(contato);

                    ViewData["MSG_S"] = "Mensagem de contato enviado com sucesso!";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var texto in listaMensagens)
                    {
                        sb.Append(texto.ErrorMessage + "<br />");
                    }

                    ViewData["MSG_E"] = sb.ToString();
                    ViewData["CONTATO"] = contato;
                }


            }
            catch (Exception e)
            {
                ViewData["MSG_E"] = "Opps! Tivemos um erro, tente novamente mais tarde!";

                //TODO - Implementar Log
            }


            return View("Contato");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CadastroCliente()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastroCliente([FromForm] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _banco.Add(cliente);
                _banco.SaveChanges();

                TempData["MSG_S"] = "Cadastro Realizado com sucesso";
                //TODO - Implementar redirecionamentos diferentes(Painel, Carrinho de compras, etc).

                return RedirectToAction(nameof(CadastroCliente));
            }
            return View();
        }

        public IActionResult CarrinhoCompras()
        {
            return View();
        }
    }
}