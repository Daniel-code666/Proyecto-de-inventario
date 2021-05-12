using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using regMVC.Models;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace regMVC.Controllers
{
    public class MostradorController : Controller
    {
        //listar y seleccionar producto en mostrador
        public ActionResult ListMostrador()
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var p = dc.productos.ToList();

                var b = dc.bodega.ToList();

                var m = dc.mostrador.ToList();

                ProductandBodViewModel finalitem = new ProductandBodViewModel();

                finalitem.prod = p;

                finalitem.bod = b;

                finalitem.mostrador = m;

                return View(finalitem);
            }
        }

        //añadir cantidad en mostrador
        public ActionResult AssignMostrador(int id)
        {
            using(inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var p = dc.productos.Where(x => x.id == id).FirstOrDefault();

                var b = dc.bodega.Where(a => a.cod_prod == p.codigo_prod).FirstOrDefault();

                var m = dc.mostrador.Where(e => e.id_bodega == b.id_bodega).FirstOrDefault();

                if(m != null)
                {
                    bodegaViewModel bod_data = new bodegaViewModel();

                    bod_data.cod_prod = b.cod_prod;
                    bod_data.fecha_ingreso = b.fecha_ingreso;
                    bod_data.cant_producto_inventario = b.cant_producto_inventario;
                    bod_data.id_bodega = b.id_bodega;
                    bod_data.cant_prod_mostrador = m.cant_prod_mostrador;

                    return View(bod_data);
                }
                else
                {
                    bodegaViewModel bod_data = new bodegaViewModel();

                    bod_data.cod_prod = b.cod_prod;
                    bod_data.fecha_ingreso = b.fecha_ingreso;
                    bod_data.cant_producto_inventario = b.cant_producto_inventario;
                    bod_data.id_bodega = b.id_bodega;

                    return View(bod_data);
                }
            }
        }

        [HttpPost]
        public ActionResult AssignMostrador(bodegaViewModel bod)
        {
            using(inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var b = dc.bodega.Where(x => x.cod_prod == bod.cod_prod).FirstOrDefault();

                if(b !=  null)
                {
                    var m = dc.mostrador.Where(a => a.id_bodega == b.id_bodega).FirstOrDefault();

                    if(m != null)
                    {
                        mostrador most = new mostrador
                        {
                            id_bodega = bod.id_bodega,
                            cant_prod_mostrador = bod.cant_prod_mostrador
                        };

                        bodega bodegaN = new bodega
                        {
                            id_bodega = bod.id_bodega,
                            fecha_ingreso = bod.fecha_ingreso,
                            cod_prod = bod.cod_prod,
                            cant_producto_inventario = bod.cant_producto_inventario
                        };

                        var e = dc.economia.Where(x => x.id_bodega == bod.id_bodega).FirstOrDefault();

                        if (most.cant_prod_mostrador > bodegaN.cant_producto_inventario)
                        {
                            Response.Write("<script>alert('La cantidad de productos asignados al mostrador exceden al total del inventario')</script>");

                            return View();
                        }
                        else
                        {
                            if (m != null)
                            {
                                if (most.cant_prod_mostrador == m.cant_prod_mostrador)
                                {
                                    dc.economia.Remove(e);

                                    dc.bodega.Remove(b);

                                    dc.mostrador.Remove(m);

                                    dc.SaveChanges();

                                    AssignMostrador2(most, bodegaN);
                                }
                                else
                                {
                                    bodegaN.cant_producto_inventario -= most.cant_prod_mostrador;

                                    dc.economia.Remove(e);

                                    dc.bodega.Remove(b);

                                    dc.mostrador.Remove(m);

                                    dc.SaveChanges();

                                    AssignMostrador2(most, bodegaN);
                                }
                            }
                            else
                            {
                                if (most.cant_prod_mostrador == m.cant_prod_mostrador)
                                {
                                    dc.economia.Remove(e);

                                    dc.bodega.Remove(b);

                                    dc.SaveChanges();

                                    AssignMostrador2(most, bodegaN);
                                }
                                else
                                {
                                    bodegaN.cant_producto_inventario -= most.cant_prod_mostrador;

                                    dc.economia.Remove(e);

                                    dc.bodega.Remove(b);

                                    dc.SaveChanges();

                                    AssignMostrador2(most, bodegaN);
                                }
                            }
                        }
                    }
                    else
                    {
                        /*mostrador most = new mostrador
                        {
                            id_bodega = bod.id_bodega,
                            cant_prod_mostrador = bod.cant_prod_mostrador
                        };

                        bodega bodegaN = new bodega
                        {
                            id_bodega = bod.id_bodega,
                            fecha_ingreso = bod.fecha_ingreso,
                            cod_prod = bod.cod_prod,
                            cant_producto_inventario = bod.cant_producto_inventario
                        };

                        dc.bodega.Remove(b);

                        dc.SaveChanges();

                        AssignMostrador2(most, bodegaN);*/
                    }
                }
            }
            return RedirectToAction("ListMostrador", "mostrador");
        }
        
        [HttpPost]
        public ActionResult AssignMostrador2(mostrador most, bodega bodegaN)
        {
            using(inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var p = dc.productos.Where(x => x.codigo_prod == bodegaN.cod_prod).FirstOrDefault() ;

                var us = System.Web.HttpContext.Current.User.Identity.Name;

                var perfil = dc.user.Where(x => x.correo == us).FirstOrDefault();

                most.cedula = perfil.cedula;

                most.cod_prod = p.codigo_prod;

                economia eco = new economia
                {
                    id_bodega = bodegaN.id_bodega,
                    cod_prod = p.codigo_prod
                };

                eco.tot_money_bodega = p.precio_compra * bodegaN.cant_producto_inventario;
                eco.tot_money_mostrador = p.precio_compra * most.cant_prod_mostrador;
                eco.dinero_total = eco.tot_money_mostrador + eco.tot_money_bodega;

                dc.mostrador.Add(most);

                dc.bodega.Add(bodegaN);

                dc.economia.Add(eco);

                dc.SaveChanges();

                return View("ListMostrador", "mostrador");
            }
        }
    }
}