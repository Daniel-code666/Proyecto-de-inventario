using regMVC.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace regMVC.Controllers
{
    public class CajaController : Controller
    {

        //muestra los productos que actualmente se encuentran en el mostrador
        public ActionResult ProdOnMostrador()
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                ProductandBodViewModel finalitem = new ProductandBodViewModel();

                var m_List = dc.mostrador.Where(x => x.cant_prod_mostrador > 0).ToList();

                if (m_List != null)
                {
                    var p_List = dc.productos.ToList();

                    finalitem.bod = m_List.SelectMany(prodEach => dc.bodega.Where(a => a.id_bodega == prodEach.id_bodega)).ToList();

                    finalitem.prod = p_List;

                    finalitem.mostrador = m_List;

                    return View(finalitem);
                }
                else
                {
                    Response.Write("<script>alert('No hay productos en el mostrador para la venta')</script>");
                    return View();
                }
            }

        }

        //venta de producto

        [HttpGet]
        public ActionResult Venta(int id)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var p = dc.productos.Where(x => x.id == id).FirstOrDefault();

                var b = dc.bodega.Where(a => a.cod_prod == p.codigo_prod).FirstOrDefault();

                var m = dc.mostrador.Where(v => v.id_bodega == b.id_bodega).FirstOrDefault();

                if (m.cant_prod_mostrador == 0)
                {
                    return RedirectToAction("CajaMsj");
                }
                else
                {
                    CajaViewModel c = new CajaViewModel
                    {
                        nombre = p.nombre,
                        precio_venta = p.precio_venta,
                        codigo_prod = b.cod_prod,
                        cant_prod_mostrador = m.cant_prod_mostrador
                    };

                    return View(c);
                }
            }
        }

        [HttpPost]
        public ActionResult Venta(CajaViewModel c)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var us = System.Web.HttpContext.Current.User.Identity.Name;

                var perfil = dc.user.Where(x => x.correo == us).FirstOrDefault();

                var p = dc.productos.Where(x => x.codigo_prod == c.codigo_prod).FirstOrDefault();

                var b = dc.bodega.Where(x => x.cod_prod == p.codigo_prod).FirstOrDefault();

                var m = dc.mostrador.Where(x => x.id_bodega == b.id_bodega).FirstOrDefault();

                var e = dc.economia.Where(x => x.id_bodega == b.id_bodega).FirstOrDefault();

                mostrador most = new mostrador
                {
                    id_bodega = b.id_bodega,
                    cant_prod_mostrador = m.cant_prod_mostrador,
                    id_mostrador = m.id_mostrador,
                    cedula = perfil.cedula
                };

                caja caja = new caja()
                {
                    nombre = c.nombre,
                    cod_prod = c.codigo_prod,
                    cantidad_vendida = c.cantidad_vendida,
                    fecha_dia = DateTime.Now,
                    fecha_mes = DateTime.Now,
                    fecha_año = DateTime.Now,
                    cedula = perfil.cedula
                };

                if (c.cantidad_vendida != null)
                {
                    most.cant_prod_mostrador -= (int)c.cantidad_vendida;

                    caja.dinero_final = c.cantidad_vendida * p.precio_venta;

                    dc.mostrador.Remove(m);

                    dc.economia.Remove(e);

                    dc.SaveChanges();

                    AddOnCaja(caja, most);

                    return RedirectToAction("ProdOnMostrador");
                }
                else
                {
                    Response.Write("<script>alert('No hay ninguna cantidad de productos para vender')</script>");

                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult AddOnCaja(caja caja, mostrador most)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var b = dc.bodega.Where(x => x.id_bodega == most.id_bodega).FirstOrDefault();

                var p = dc.productos.Where(x => x.codigo_prod == b.cod_prod).FirstOrDefault();

                most.cedula = caja.cedula;

                most.cod_prod = p.codigo_prod;

                economia eco = new economia
                {
                    id_bodega = most.id_bodega,
                    cod_prod = p.codigo_prod                    
                };

                eco.tot_money_bodega = p.precio_compra * b.cant_producto_inventario;
                eco.tot_money_mostrador = p.precio_compra * most.cant_prod_mostrador;
                eco.dinero_total = eco.tot_money_mostrador + eco.tot_money_bodega;

                dc.caja.Add(caja);

                dc.mostrador.Add(most);

                dc.economia.Add(eco);

                dc.SaveChanges();

                return RedirectToAction("ProdOnMostrador");
            }
        }

        //lista global de ventas

        public ActionResult ListVentas()
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                return View(dc.caja.ToList());
            }
        }

        //muestra el resultado del filtro de ventas dentro de dos fechas

        public ActionResult ListVentas2(DateTime start, DateTime end)
        {
            bool Status = false;

            if(start == null || end == null)
            {
                Response.Write("<script>alert('Debe la fecha inicial y la fecha final para aplicar el filtro')</script>");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
                    {
                        var d = dc.caja.Where(x => x.fecha_dia >= start && x.fecha_dia <= end).ToList();

                        Status = true;

                        ViewBag.Status = Status;

                        ViewBag.start = start;

                        ViewBag.end = end;

                        return View(d);
                    }
                }
            }
            return View();
        }

        //importa a un pdf de las ventas globales

        public ActionResult SalesToPdf()
        {
            var reporte = new ActionAsPdf("ListVentas");

            return reporte;
        }

        //importa un pdf las ventas realizadas dentro de dos fechas

        public ActionResult SalesToPdf2(DateTime start, DateTime end)
        {
            var reporte = new ActionAsPdf("ListVentas2", new {start, end});

            return reporte;
        }

        //genera el gráfico de ventas

        public ActionResult SalesChart()
        {
            using(inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var c = dc.caja.ToList();

                List<string> prod_name = new List<string>();

                List<int> cantidad = new List<int>();

                var label_name = c.Select(x => x.nombre);

                var cant_sell = c.Select(x => x.cantidad_vendida);

                /*foreach(var item in label_name)
                {
                    prod_name.Add(c.Count(x => x.nombre == item).ToString());
                }

                foreach(var item2 in cant_sell)
                {
                    cantidad.Add(c.Count(x => x.cantidad_vendida == item2));
                }*/

                var rep = prod_name;

                var rep2 = cant_sell;

                ViewBag.label_name = label_name;

                ViewBag.rep = prod_name.ToList();

                //ViewBag.cant_sell = cant_sell;

                ViewBag.rep2 = cant_sell.ToList();

                return View();

            }
        }

        //importa el gráfico de ventas a una img

        public ActionResult SalesChartToImage()
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var reporte = new ActionAsImage("SalesChart");

                return reporte;
            }
        }

        //Mensaje cuando se la cantidad del producto es 0 en el mostrador

        public ActionResult CajaMsj()
        {
            return View();
        }
    }
}