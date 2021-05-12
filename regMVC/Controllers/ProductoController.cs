using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Ajax.Utilities;
using regMVC.Models;
using Rotativa;

namespace regMVC.Controllers
{
    //registrar, lista de categoría y grupo
    public class ProductoController : Controller
    {
        //registrar
        [HttpGet]
        public ActionResult prodReg()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem() { Text = "Aseo", Value = "aseo" });
            lst.Add(new SelectListItem() { Text = "Frutas", Value = "frutas" });
            lst.Add(new SelectListItem() { Text = "Verduras", Value = "verduras" });
            lst.Add(new SelectListItem() { Text = "Paquetes", Value = "paquetes" });
            lst.Add(new SelectListItem() { Text = "Alimentos en botellas", Value = "botellas" });
            lst.Add(new SelectListItem() { Text = "Utencilios de limpieza", Value = "limpiezaTools" });

            List<SelectListItem> lst2 = new List<SelectListItem>();
            lst2.Add(new SelectListItem() { Text = "Ninguno", Value = "noAlimento" });
            lst2.Add(new SelectListItem() { Text = "Perecedero", Value = "perecedero" });
            lst2.Add(new SelectListItem() { Text = "No perecedero", Value = "noPerecedero" });

            ViewBag.categoria = lst;

            ViewBag.grupo = lst2;

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult prodReg(productos prod)
        {
            bool Status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                var exist = CheckCod(prod.codigo_prod);

                using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
                {
                    var us = System.Web.HttpContext.Current.User.Identity.Name;

                    var perfil = dc.user.Where(x => x.correo == us).FirstOrDefault();

                    prod.cedula = perfil.cedula;

                    if (exist)
                    {
                        return RedirectToAction("ProdMsj", "producto");
                    }

                    bodega bod = new bodega()
                    {
                        cod_prod = prod.codigo_prod,
                        fecha_ingreso = prod.fecha_ingreso,
                        cant_producto_inventario = prod.cant_producto_inventario
                    };

                    mostrador most = new mostrador
                    {
                        id_bodega = bod.id_bodega,
                        cant_prod_mostrador = 0,
                        cod_prod = prod.codigo_prod
                    };

                    most.cedula = perfil.cedula;
                    
                    var costoBod = prod.precio_compra * bod.cant_producto_inventario;

                    var costoMost = prod.precio_compra * most.cant_prod_mostrador;

                    var costoTot = costoBod + costoMost;

                    economia eco = new economia
                    {
                        id_bodega = bod.id_bodega,
                        tot_money_bodega = costoBod,
                        tot_money_mostrador = costoMost,
                        dinero_total = costoTot,
                        cod_prod = prod.codigo_prod
                    };

                    dc.bodega.Add(bod);

                    dc.mostrador.Add(most);

                    dc.productos.Add(prod);

                    dc.economia.Add(eco);

                    dc.SaveChanges();

                    message = "Registro completado";

                    Status = true;
                }
            }
            else
            {
                message = "Petición inválida";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;

            return View();
        }

        //listar

        public ActionResult prodList()
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var p = dc.productos.ToList();

                var b = dc.bodega.ToList();

                ProductandBodViewModel finalitem = new ProductandBodViewModel();

                finalitem.prod = p;

                finalitem.bod = b;

                return View(finalitem);
            }
        }

        //generar pdf de inventario
        public ActionResult ProdListToPdf()
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var reporte = new ActionAsPdf("prodList");

                return reporte;
            }
        }

        //editar
        public ActionResult prodEdit(int id)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                List<SelectListItem> lst = new List<SelectListItem>();
                lst.Add(new SelectListItem() { Text = "Aseo", Value = "aseo" });
                lst.Add(new SelectListItem() { Text = "Frutas", Value = "frutas" });
                lst.Add(new SelectListItem() { Text = "Verduras", Value = "verduras" });
                lst.Add(new SelectListItem() { Text = "Paquetes", Value = "paquetes" });
                lst.Add(new SelectListItem() { Text = "Alimentos en botellas", Value = "botellas" });
                lst.Add(new SelectListItem() { Text = "Utencilios de limpieza", Value = "limpiezaTools" });

                List<SelectListItem> lst2 = new List<SelectListItem>();
                lst2.Add(new SelectListItem() { Text = "Ninguno", Value = "noAlimento" });
                lst2.Add(new SelectListItem() { Text = "Perecedero", Value = "perecedero" });
                lst2.Add(new SelectListItem() { Text = "No perecedero", Value = "noPerecedero" });

                ViewBag.categoria = lst;

                ViewBag.grupo = lst2;

                var u = dc.productos.Where(a => a.id == id).FirstOrDefault();

                if (u != null)
                {
                    var pm = new productos
                    {
                        codigo_prod = u.codigo_prod,
                        cant_producto_inventario = u.cant_producto_inventario,
                        fecha_ingreso = u.fecha_ingreso
                    };

                    var b = dc.bodega.Where(x => x.cod_prod == pm.codigo_prod).FirstOrDefault();

                    u.codigo_prod = b.cod_prod;
                    u.fecha_expiracion = u.fecha_expiracion;
                    u.cant_producto_inventario = b.cant_producto_inventario;
                    u.fecha_ingreso = b.fecha_ingreso;

                    return View(u);
                }

                return Content("Producto inválido");

            }
        }

        [HttpPost]
        public ActionResult prodEdit(productos prod)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var u = dc.productos.Where(a => a.id == prod.id).FirstOrDefault();

                if (u != null)
                {
                    var pm = new productos
                    {
                        codigo_prod = prod.codigo_prod,
                        cant_producto_inventario = prod.cant_producto_inventario,
                        fecha_ingreso = prod.fecha_ingreso
                    };

                    var b = dc.bodega.Where(x => x.cod_prod == u.codigo_prod).FirstOrDefault();

                    if (b != null)
                    {

                        bodega bod = new bodega()
                        {
                            id_bodega = b.id_bodega,
                            fecha_ingreso = pm.fecha_ingreso,
                            cant_producto_inventario = pm.cant_producto_inventario,
                            cod_prod = pm.codigo_prod
                        };

                        var m = dc.mostrador.Where(a => a.id_bodega == b.id_bodega).FirstOrDefault();

                        var e = dc.economia.Where(i => i.id_bodega == b.id_bodega).FirstOrDefault();

                        if (m != null)
                        {
                            mostrador most = new mostrador()
                            {
                                id_bodega = b.id_bodega,
                                cant_prod_mostrador = m.cant_prod_mostrador,
                                id_mostrador = m.id_mostrador
                            };

                            if (bod.cod_prod != null)
                            {
                                dc.mostrador.Remove(m);
                                dc.bodega.Remove(b);
                                dc.productos.Remove(u);
                                dc.economia.Remove(e);

                                dc.SaveChanges();

                                prodEdit2(prod, bod, most);
                            }
                        }
                        else
                        {
                            if (bod.cod_prod != null)
                            {
                                dc.bodega.Remove(b);
                                dc.productos.Remove(u);
                                dc.economia.Remove(e);
                                dc.SaveChanges();

                                mostrador most = new mostrador()
                                {
                                    id_bodega = null,
                                    cant_prod_mostrador = 0,
                                    id_mostrador = 0
                                };

                                prodEdit2(prod, bod, most);
                            }
                        }
                    }
                }
                return RedirectToAction("prodList", "producto");
            }
        }

        [HttpPost]
        public ActionResult prodEdit2(productos p, bodega bod, mostrador most)
        {
            var exist = CheckCod(p.codigo_prod);

            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var us = System.Web.HttpContext.Current.User.Identity.Name;

                var perfil = dc.user.Where(x => x.correo == us).FirstOrDefault();

                if (exist)
                {
                    return RedirectToAction("ProdMsj2", "producto");
                }

                p.cedula = perfil.cedula;

                economia eco = new economia
                {
                    id_bodega = bod.id_bodega,
                    cod_prod = p.codigo_prod
                };

                eco.tot_money_bodega = p.precio_compra * bod.cant_producto_inventario;
                eco.tot_money_mostrador = p.precio_compra * most.cant_prod_mostrador;
                eco.dinero_total = eco.tot_money_mostrador + eco.tot_money_bodega;


                dc.productos.Add(p);
                dc.bodega.Add(bod);
                dc.economia.Add(eco);

                if (most.id_mostrador == 0)
                {
                    mostrador mostrA = new mostrador()
                    {
                        id_bodega = bod.id_bodega,
                        cod_prod = p.codigo_prod
                    };

                    mostrA.cedula = perfil.cedula;

                    dc.mostrador.Add(mostrA);
                }
                else
                {
                    mostrador mostrB = new mostrador
                    {
                        id_bodega = bod.id_bodega,
                        cant_prod_mostrador = most.cant_prod_mostrador,
                        id_mostrador = most.id_mostrador,
                        cod_prod = p.codigo_prod
                    };

                    mostrB.cedula = perfil.cedula;

                    dc.mostrador.Add(mostrB);
                }

                dc.SaveChanges();

                return RedirectToAction("prodList", "producto");
            }
        }

        //eliminar

        public ActionResult prodDel(int id)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var u = dc.productos.Where(a => a.id == id).FirstOrDefault();

                if (u != null)
                {
                    var pm = new productos
                    {
                        nombre = u.nombre,
                        marca = u.marca,
                        precio_compra = u.precio_compra,
                        descripcion = u.descripcion,
                        categoria = u.categoria,
                        precio_venta = u.precio_venta,
                        grupo = u.grupo,
                        codigo_prod = u.codigo_prod,
                        fecha_expiracion = u.fecha_expiracion,
                        cant_producto_inventario = u.cant_producto_inventario,
                        fecha_ingreso = u.fecha_ingreso
                    };

                    var b = dc.bodega.Where(x => x.cod_prod == pm.codigo_prod).FirstOrDefault();

                    u.codigo_prod = b.cod_prod;
                    u.fecha_expiracion = u.fecha_expiracion;
                    u.cant_producto_inventario = b.cant_producto_inventario;
                    u.fecha_ingreso = b.fecha_ingreso;

                    return View(u);
                }

                return View(u);
            }
        }

        [HttpPost]
        public ActionResult prodDel(productos prod)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var u = dc.productos.Where(a => a.id == prod.id).FirstOrDefault();

                var b = dc.bodega.Where(x => x.cod_prod == u.codigo_prod).FirstOrDefault();

                var m = dc.mostrador.Where(z => z.id_bodega == b.id_bodega).FirstOrDefault();

                var e = dc.economia.Where(x => x.id_bodega == b.id_bodega).FirstOrDefault();

                dc.economia.Remove(e);
                dc.mostrador.Remove(m);
                dc.bodega.Remove(b);
                dc.productos.Remove(u);

                dc.SaveChanges();

                return RedirectToAction("prodList", "producto");
            }
        }

        //detalles 

        public ActionResult prodDetails(productos prod)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var p = dc.productos.Where(a => a.id == prod.id).FirstOrDefault();

                var b = dc.bodega.Where(a => a.cod_prod == p.codigo_prod).FirstOrDefault();

                var m = dc.mostrador.Where(a => a.id_bodega == b.id_bodega).FirstOrDefault();

                var c = dc.caja.Where(a => a.cod_prod == p.codigo_prod).FirstOrDefault();

                if (p != null)
                {
                    ProductDetails productDetails = new ProductDetails
                    {
                        id = p.id,
                        nombre = p.nombre,
                        marca = p.marca,
                        precio_compra = p.precio_compra,
                        descripcion = p.descripcion,
                        categoria = p.categoria,
                        precio_venta = p.precio_venta,
                        grupo = p.grupo,
                        codigo_prod = p.codigo_prod,
                        fecha_expiracion = p.fecha_expiracion,
                        cant_producto_inventario = b.cant_producto_inventario,
                        fecha_ingreso = b.fecha_ingreso,
                        cant_prod_mostrador = m.cant_prod_mostrador
                    };

                    return View(productDetails);
                }

                return View();
            }
        }

        //generar pdf de producto

        public ActionResult DetailsToPdf(int id)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var p = dc.productos.Where(a => a.id == id).FirstOrDefault();

                var b = dc.bodega.Where(a => a.cod_prod == p.codigo_prod).FirstOrDefault();

                var m = dc.mostrador.Where(a => a.id_bodega == b.id_bodega).FirstOrDefault();

                ProductDetails pro = new ProductDetails
                {
                    id = p.id,
                    nombre = p.nombre,
                    marca = p.marca,
                    precio_compra = p.precio_compra,
                    descripcion = p.descripcion,
                    categoria = p.categoria,
                    precio_venta = p.precio_venta,
                    grupo = p.grupo,
                    codigo_prod = p.codigo_prod,
                    fecha_expiracion = p.fecha_expiracion,
                    cant_producto_inventario = b.cant_producto_inventario,
                    fecha_ingreso = b.fecha_ingreso,
                    cant_prod_mostrador = m.cant_prod_mostrador
                };

                var reporte = new ViewAsPdf("~/Views/producto/prodDetails.cshtml", pro);
                return reporte;

            }
        }

        //listar por categoría

        private List<SelectListItem> buildCategories(string selectedItem = "")
        {
            //método para crear los elementos del dropdownlist
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "Aseo", Value = "aseo" });
            lst.Add(new SelectListItem() { Text = "Frutas", Value = "frutas" });
            lst.Add(new SelectListItem() { Text = "Verduras", Value = "verduras" });
            lst.Add(new SelectListItem() { Text = "Paquetes", Value = "paquetes" });
            lst.Add(new SelectListItem() { Text = "Alimentos en botellas", Value = "botellas" });
            lst.Add(new SelectListItem() { Text = "Utencilios de limpieza", Value = "limpiezaTools" });


            if (!string.IsNullOrEmpty(selectedItem))
            {
                lst.Find(x => x.Value.ToLower() == selectedItem.ToLower()).Selected = true;
            }
            return lst;
        }

        private List<productos> listaProd()
        {
            //método para convertir los elementos que coincidan
            //con la categoría seleccionada a una lista
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var producto = dc.productos.ToList();

                return producto;
            }
        }

        [HttpGet]
        public ActionResult listByCat()
        {
            prodCategory listByCatViewModel = new prodCategory();
            listByCatViewModel.Categories = buildCategories();

            return View(listByCatViewModel);
        }


        [HttpPost]
        public ActionResult listByCat(prodCategory listByCatViewModel)
        {

            if (ModelState.IsValid)
            {
                if (listByCatViewModel.categoriaSeleccionada != null)
                {
                    //enlazando el dropdownlist de nuevo
                    listByCatViewModel.Categories = buildCategories(listByCatViewModel.categoriaSeleccionada);

                    using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
                    {
                        //llama al método listaprod
                        //var products = listaProd();

                        var products = listaProd();

                        //busca la categoría indicada dentro de la lista creada que contiene los 
                        //elementos de la base de datos
                        var results = products.FindAll(x => x.categoria.ToLower() ==
                                           listByCatViewModel.categoriaSeleccionada.ToLower());

                        listByCatViewModel.SearchResults = results;


                    }
                }
            }
            return View(listByCatViewModel);
        }

        //listar por grupo

        private List<SelectListItem> buildGroups(string selectedItem = "")
        {
            //método para crear los elementos del dropdownlist
            List<SelectListItem> lst2 = new List<SelectListItem>();

            lst2.Add(new SelectListItem() { Text = "Ninguno", Value = "noAlimento" });
            lst2.Add(new SelectListItem() { Text = "Perecedero", Value = "perecedero" });
            lst2.Add(new SelectListItem() { Text = "No perecedero", Value = "noPerecedero" });


            if (!string.IsNullOrEmpty(selectedItem))
            {
                lst2.Find(x => x.Value.ToLower() == selectedItem.ToLower()).Selected = true;
            }
            return lst2;
        }

        private List<productos> listaGroup()
        {
            //método para convertir los elementos que coincidan
            //con la categoría seleccionada a una lista
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var producto = dc.productos.ToList();
                return producto;
            }
        }

        [HttpGet]
        public ActionResult listByGroup()
        {
            prodGroup listByGroupViewModel = new prodGroup();
            listByGroupViewModel.grupos = buildGroups();

            return View(listByGroupViewModel);
        }

        [HttpPost]
        public ActionResult listByGroup(prodGroup listByGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                if (listByGroupViewModel.grupoSeleccionado != null)
                {
                    //enlazando el dropdownlist de nuevo
                    listByGroupViewModel.grupos = buildGroups(listByGroupViewModel.grupoSeleccionado);

                    using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
                    {
                        //llama al método listaprod
                        var products = listaGroup();

                        //busca la categoría indicada dentro de la lista creada que contiene los 
                        //elementos de la base de datos
                        var results = products.FindAll(x => x.grupo.ToLower() ==
                                           listByGroupViewModel.grupoSeleccionado.ToLower());
                        listByGroupViewModel.ResultadosBusqueda = results;
                    }
                }
            }
            return View(listByGroupViewModel);
        }

        //buscar producto

        public ActionResult search(string search)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var p = dc.productos.Where(x => x.codigo_prod.ToLower().Contains(search.ToLower()) 
                || x.fecha_expiracion.ToString().Contains(search)
                || x.nombre.ToLower().Contains(search.ToLower()) || x.marca.ToLower().Contains(search.ToLower()) 
                || x.precio_compra.ToString().Contains(search) 
                || x.precio_venta.ToString().Contains(search)
                || x.categoria.ToString().Contains(search)
                || x.grupo.ToString().Contains(search));

                var b = dc.bodega.Where(x => x.fecha_ingreso.ToString().Contains(search));

                /*ProductandBodViewModel finalitem = new ProductandBodViewModel();

                finalitem.prod = p.ToList();

                finalitem.bod = b.ToList();
                */
                return View(p.ToList());
            }
        }
        
        //Mensaje por si se repite el código del producto durante el registro

        public ActionResult ProdMsj()
        {
            return View();
        }

        //Mensaje por si se repite el código del producto durante editar

        public ActionResult ProdMsj2()
        {
            return View();
        }

        //verificar que el código del producto no sea el mismo

        [NonAction]
        public bool CheckCod(string codigo_prod)
        {
            using(inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var c = dc.productos.Where(x => x.codigo_prod == codigo_prod).FirstOrDefault();

                return c != null;
            }
        }

    }
}