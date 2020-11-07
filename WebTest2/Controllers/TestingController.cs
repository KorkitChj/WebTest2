using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTest2.Models;

namespace WebTest2.Controllers
{
    public class TestingController : Controller
    {
        // GET: Testing
        // print all module
        //query_list_module()
        [HttpGet]
        public ActionResult Index()
        {
            using (var context = new DBModels())
            {

                // Return the list of data from the database
                var data = context.list_module_name_table.ToList();              
                return View(data);
            }
        }

        // and new module
        public ActionResult add_new_module()
        {
            return View();
        }
        //save_test_case()
        [HttpPost]
        public ActionResult add_new_module(list_module_name_table model)
        {
            //check submited data is available
            //check data is required
            /*The ModelState represents a collection of name and value pairs that were 
             *submitted to the server during a POST. It also 
             *contains a collection of error messages for each value submitted.
             */
            String status = "false";
            String message = "";
            if (ModelState.IsValid)
            {               
                using (var context = new DBModels())
                {
                    try
                    {
                        bool res = check_duplicate_test_case_name(model.name_of_module);
                        if (res)
                        {
                            ViewBag.status = "false";
                            ViewBag.Message = "Already have modules";
                            return View();
                        }
                        else
                        {                            
                            //add module to list_module
                            context.list_module_name_table.Add(model);
                            context.SaveChanges();

                            status = "true";
                            message = "Successfully";
                        }
                        
                    }
                    catch(Exception ex)
                    {
                        status = "false";
                        message = ex.Message;
                    }                                  
                }
                /*
                 * The ViewBag in ASP.NET MVC is used to transfer temporary data 
                 * (which is not included in the model) from the controller to the view.
                 */
            
            }
            /*TempData is a dictionary which is derived from TempDataDictionary class. 
             * TempData is stored data just like live session for short time. 
             * TempData keeps data for the time of HTTP Request, 
             * which means that it holds data between two consecutive requests. 
             */
            if(status == "true")
            {
                TempData["Message"] = message;
                TempData["Status"] = status;
                return RedirectToAction("Index", model);
            }
            else
            {
                ViewBag.Message = message;
                ViewBag.Status = status;              
                return View();
            }
            
        }

        public ActionResult DelModule(int id, string module_name)
        {
            String status = "false";
            String message = "";
            using (var context = new DBModels())
            {
                try
                {
                    var itemToRemove = context.list_module_name_table.SingleOrDefault(x => x.module_id == id && x.name_of_module == module_name);

                    if (itemToRemove != null)
                    {
                        context.list_module_name_table.Remove(itemToRemove);
                        context.SaveChanges();
                        status = "true";
                        message = "Successfully";
                    }
                    else
                    {
                        TempData["Message"] = "module_id && module_name not found";
                        TempData["Status"] = status;
                        //ยังไม่ส่งอะไรไปแสดง
                        return RedirectToAction("Index");
                    }

                   
                }
                catch (Exception ex)
                {
                    status = "false";
                    message = ex.Message;
                }
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("Index");
        }

        //check_duplicate_test_case_name()
        public Boolean check_duplicate_test_case_name(string case_name)
        {
            using (var context = new DBModels())
            {
                bool result = context.list_module_name_table.Any(u => u.name_of_module == case_name);
                return result;
            }
            
        }

        //query_rtm_data()
        //receive key of module add redirect to  RTM page
        [HttpGet]
        public ActionResult LinkRTM(int id,string module_name)
        {
            
            using (var context = new DBModels())
            {
                //string query_rtm_table = "SELECT * FROM rtm_table WHERE module_id=@id";
                //var rtm_data = context.rtm_table.SqlQuery(query_rtm_table, new SqlParameter("@id", id)).ToList();

                //LINQ Syntax
                var rtm_data = context.rtm_table.Where(s => s.module_id == id).ToList();
            
            
            //var data = context.rtm_table.ToList();
            ViewBag.Module_id = id;
            ViewBag.MName = module_name;
             return View(rtm_data);
            }
            
        }

        public ActionResult del_rtm(int id,int module_id, string module_name)
        {
            String status = "false";
            String message = "";
            using (var context = new DBModels())
            {
                try
                {
                    var itemToRemove = context.rtm_table.SingleOrDefault(x => x.req_id == id && x.module_id == module_id);

                    if (itemToRemove != null)
                    {
                        context.rtm_table.Remove(itemToRemove);
                        context.SaveChanges();
                        status = "true";
                        message = "Successfully";
                    }
                    else
                    {
                        TempData["Message"] = "req_id && module_id not found";
                        TempData["Status"] = status;
                        //ยังไม่ส่งอะไรไปแสดง
                        return RedirectToAction("Index");
                    }

                    
                }
                catch (Exception ex)
                {
                    status = "false";
                    message = ex.Message;
                }
            }
            TempData["MessageRTM"] = message;
            TempData["StatusRTM"] = status;
            return RedirectToAction("LinkRTM", new { id = module_id, module_name = module_name });
        }

        [HttpGet]
        //edit module id
        public ActionResult EditRTM(int id, string module_name)
        {
            using (var context = new DBModels())
            {
                ViewBag.Module_id = id;
                ViewBag.MName = module_name;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditRTM(list_module_name_table model)
        {          
            String status = "false";
            String message = "";
            if (ModelState.IsValid)
            {
                using (var context = new DBModels())
                {
                    try
                    {
                        bool res = check_duplicate_test_case_name(model.name_of_module);
                        if (res)
                        {
                            ViewBag.status = "false";
                            ViewBag.Message = "Already have modules";
                            return View();
                        }
                        else
                        {
                            var result = context.list_module_name_table.First(a => a.module_id == model.module_id);
                            if (result != null)
                            {
                                result.name_of_module = model.name_of_module;
                                context.SaveChanges();
                                status = "true";
                                message = "Successfully";
                            }                           

                            
                        }

                    }
                    catch (Exception ex)
                    {
                        status = "false";
                        message = ex.Message;
                    }
                }
            }           
            if (status == "true")
            {
                TempData["Message"] = message;
                TempData["Status"] = status;
                return RedirectToAction("Index", model);
            }
            else
            {
                ViewBag.Message = message;
                ViewBag.Status = status;
                return View();
            }

        }

        [HttpGet]
        public ActionResult add_new_rtm(int id, string module_name)
        {
            ViewBag.Module_id = id;
            ViewBag.MName = module_name;
            //var rtm_table = new rtm_table();
            return View();
        }
        //save_rtm_data()
        [HttpPost]
        public ActionResult add_new_rtm(rtm_table model)
        {   
            //add new row rtm to database            
            String status = "false";
            String message = "";
            int module_id = model.module_id;
            string module_name1 = "";            
            using (var context = new DBModels())
            {               
                if (ModelState.IsValid)
                {
                    var module_name = context.list_module_name_table.Where(s => s.module_id == module_id).ToList();
                    if (module_name.Count > 0)
                    {
                        module_name1 = module_name[0].name_of_module;
                    }
                    try
                    {
                        
                        context.rtm_table.Add(model);
                        context.SaveChanges();

                        status = "true";
                        message = "Successfully";
                    }
                    catch(Exception ex)
                    {
                        ViewBag.Message = ex.Message;
                        ViewBag.Status = status;
                        return View(model);
                    }
                    TempData["MessageRTM"] = message;
                    TempData["StatusRTM"] = status;
                    return RedirectToAction("LinkRTM", new { id = module_id, module_name = module_name1 });
                }
                else
                {
                    return View(model);
                }               
            }
        }

        //query_factor_name()
        [HttpGet]
        public ActionResult test_scenario(int id, string module_name)
        {
            using (var context = new DBModels())
            {
                ViewBag.Module_id = id;
                ViewBag.MName = module_name;
                //var test_scenario_data = context.factors_name_table.ToList();
                //อันเดิมที่ใช้
                //var test_scenario_data = context.factors_name_table.Where(s => s.module_id == id).ToList();
                //total factors
                int cal = totalFactors(id);

                //select item in factor
                //การ join ตาราง แล้วส่งไปยัง view เป็นหน้าว่างๆ
                //List<factors_name_table> factors_name = context.factors_name_table.ToList();
                //List<items_in_factor_table> items = context.items_in_factor_table.ToList();

                var data_item = (from a in context.factors_name_table
                                 join b in context.items_in_factor_table
                                 on a.fn_id equals b.fn_id // inner join
                                 where a.module_id == id
                                 orderby a.fn_name
                                 select new factors_items
                                 {
                                     factors_Name = a.fn_name,
                                     items_In_Factor = b.item
                                 }).ToList();
                //เปลี่ยน
                var data_item2 = (from a in context.factors_name_table
                                 join b in context.items_in_factor_table
                                 on a.fn_id equals b.fn_id into g
                                 from b in g.DefaultIfEmpty() // left join
                                 where a.module_id == id
                                 orderby a.fn_name
                                 select new factors_items
                                 {
                                     factors_Name = a.fn_name,
                                     items_In_Factor = b.item
                                 }).ToList();


                /*var data_item_3 = (
                                from a in context.factors_name_table
                                join b in context.items_in_factor_table
                                on a.fn_id equals b.fn_id
                                where a.module_id == id
                                select new { a.fn_name, b.item } into x
                                group x by new { x.fn_name,x.item } into g
                                select new factors_items
                                {
                                    factors_Name = g.Key.fn_name,
                                    items_In_Factor = g.Key.item
                                }).ToList();*/

                //List<String> data_item2 = new List<String>();
                //if (data_item.Count > 0)
                //{
                /*foreach (var item2 in data_item){                        
                    data_item2.Add(item2.item);
                    break;
                }*/

                /*for(int i = 0; i <= data_item.Count;i++)
                {
                    if (i == 0) { data_item2.Add(data_item[i].item);continue; }
                    if (data_item[i].fn_name == data_item[i].fn_name)
                    {
                        if(data_item2[i] != data_item[i].fn_name)
                        {
                            data_item2.Add(data_item[i].item+",");
                        }
                        else
                        {
                            data_item2.Add(data_item[i].item);
                        }

                    }
                }*/




                //data_item2.Add(item2.item);
                //Debug.WriteLine(data_item2);
                //}


                /*for (int runs = 0; runs <= data_item.Count; runs++)
                {
                    if ()
                    {

                    }

                }*/

                /*foreach (var fn_id_data in test_scenario_data)
                {                   
                    Debug.WriteLine(fn_id_data.fn_id);
                    var test = context.items_in_factor_table.Where(s => s.fn_id == fn_id_data.fn_id).ToList();
                }*/

                //ViewData["FactorData"] = data_item;
                ViewBag.TotalFactor = cal;
                //ViewBag.Data_item = data_item;
                return View(data_item2);
            }
            
        }

        public int totalFactors(int id)
        {
            int cal = 1;
            using (var context = new DBModels())
            {
                var data = (from a in context.factors_name_table
                            join b in context.items_in_factor_table
                            on a.fn_id equals b.fn_id
                            where a.module_id == id
                            group a by new { a.fn_id } into g
                            orderby g.Count() descending
                            select new
                            {
                                count = g.Count()
                            }).ToList();
                
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {

                        cal *= item.count;
                        //Debug.WriteLine(cal);
                    }
                }
                else
                {
                    cal = 0;
                }
            }
            
            return cal;
        }
        [HttpGet]
        public ActionResult add_new_factors(int id, string module_name)
        {
            using (var context = new DBModels())
            {
                //var test_scenario_data = context.factors_name_table.Where(s => s.module_id == id).ToList();
                ViewBag.Module_id = id;
                ViewBag.MName = module_name;
                return View();
            }
                     
        }
        //save_data_factor()
        [HttpPost]
        public ActionResult add_new_factors(factors_name_table model)
        {
            String status = "false";
            String message = "form is not valid";
            int module_id = model.module_id;
            string module_name1 = "";
            if (ModelState.IsValid)
            {
                using (var context = new DBModels())
                {
                    var module_name = context.list_module_name_table.Where(s => s.module_id == module_id).ToList();
                    if (module_name.Count > 0)
                    {
                        module_name1 = module_name[0].name_of_module;
                    }
                    try
                    {
                        bool res = check_duplicate_factors_name(model.fn_name, module_id);
                        if (res)
                        {
                            ViewBag.status = "false";
                            ViewBag.Message = "Already have Factor";
                            return View();
                        }
                        else
                        {
                            //add factor name to factor table
                            context.factors_name_table.Add(model);
                            context.SaveChanges();

                            status = "true";
                            message = "Successfully";
                        }

                    }
                    catch (Exception ex)
                    {
                        status = "false";
                        message = ex.Message;
                    }
                }             
            }
            if (status == "true")
            {
                TempData["MessageTC"] = message;
                TempData["StatusTC"] = status;
                return RedirectToAction("test_scenario", new {id = module_id,module_name = module_name1 });
            }
            else
            {
                ViewBag.Message = message;
                ViewBag.Status = status;
                return View();
            }
        }

        public ActionResult ViewFactors(int id, string module_name)
        {
            using (var context = new DBModels())
            {
                int cal = totalFactors(id);
                ViewBag.TotalFactor = cal;
                ViewBag.Module_id = id;
                ViewBag.MName = module_name;
                //var test_scenario_data = context.factors_name_table.ToList();
                //อันเดิมที่ใช้
                var test_scenario_data = context.factors_name_table.Where(s => s.module_id == id).ToList();
                return View(test_scenario_data);
            }
                
        }

        public ActionResult Delete_Factor(int id, int module_id,string module_name)
        {
            String status = "false";
            String message = "";                 
                using (var context = new DBModels())
                {                   
                    try
                    {
                            var itemToRemove = context.factors_name_table.SingleOrDefault(x => x.fn_id == id);

                            if (itemToRemove != null)
                            {
                                context.factors_name_table.Remove(itemToRemove);
                                context.SaveChanges();
                                status = "true";
                                message = "Successfully";
                            }
                            else
                            {                                
                                status = "false";
                                message = "fn_id not found";                               
                            }            

                        
                    }
                    catch (Exception ex)
                    {
                        status = "false";
                        message = ex.Message;
                    }
                }        
                TempData["MessageTC"] = message;
                TempData["StatusTC"] = status;
                return RedirectToAction("ViewFactors", new { id = module_id, module_name = module_name });          
        }

        public ActionResult del_item_in_factor(int id,int fn_id, int module_id, string module_name)
        {
            String status = "false";
            String message = "";
            using (var context = new DBModels())
            {
                try
                {
                    var itemToRemove = context.items_in_factor_table.SingleOrDefault(x => x.iif_id == id && x.fn_id == fn_id);

                    if (itemToRemove != null)
                    {
                        context.items_in_factor_table.Remove(itemToRemove);
                        context.SaveChanges();
                        status = "true";
                        message = "Successfully";
                    }
                    else
                    {
                        status = "false";
                        message = "iff_id not found";
                    }

                    
                }
                catch (Exception ex)
                {
                    status = "false";
                    message = ex.Message;
                }
            }
            TempData["MessageIF"] = message;
            TempData["StatusIF"] = status;
            return RedirectToAction("query_items_in_factor", new { id = fn_id, module_id = module_id, module_name = module_name });
        }
        //check_duplicate_factor()
        public Boolean check_duplicate_factors_name(string factor_name,int module_id)
        {
            using (var context = new DBModels())
            {
                bool result = context.factors_name_table.Any(u => u.fn_name == factor_name && u.module_id == module_id);
                return result;
            }

        }
        //query_items_in_factor()
        public ActionResult query_items_in_factor(int id,int module_id,string module_name)
        {
            using (var context = new DBModels())
            {
                ViewBag.Module_id = module_id;
                ViewBag.MName = module_name;
                ViewBag.Fnid = id;
                var result = context.items_in_factor_table.Where(u => u.fn_id == id).ToList();
                return View(result);
            }
        }
        [HttpGet]
        //save_items_in_factor()
        public ActionResult save_items_in_factor(int id, string module_name, int fn_id)
        {
            using (var context  = new DBModels())
            {
                ViewBag.Module_id = id;
                ViewBag.MName = module_name;
                ViewBag.Fnid = fn_id;
                return View();
            }
        }

        [HttpPost]
        //save_items_in_factor()
        public ActionResult save_items_in_factor(items_in_factor_table model)
        {
            String status = "false";
            String message = "";
            int fn_id = model.fn_id;
            string module_name = "";
            int module_id = 0;
            if (ModelState.IsValid)
            {
                using (var context = new DBModels())
                {                   
                    var factor_data = context.factors_name_table.Where(s => s.fn_id == fn_id).ToList();
                    if (factor_data.Count > 0)
                    {                       
                        module_id = factor_data[0].module_id;
                        var module_name2 = context.list_module_name_table.Where(s => s.module_id == module_id).ToList();
                        module_name = module_name2[0].name_of_module;
                    }
                    try
                    {
                        bool res = check_duplicate_items_factors(model.item,model.fn_id);
                        if (res)
                        {
                            ViewBag.Status = "false";
                            ViewBag.Message = "Already have Item Factor";
                            return View();
                        }
                        else
                        {
                            //add factor name to factor table
                            context.items_in_factor_table.Add(model);
                            context.SaveChanges();

                            status = "true";
                            message = "Successfully";
                        }

                    }
                    catch (Exception ex)
                    {
                        status = "false";
                        message = ex.Message;
                    }
                }
            }
            if (status == "true")
            {
                TempData["MessageIF"] = message;
                TempData["StatusIF"] = status;
                return RedirectToAction("query_items_in_factor", new { id = model.fn_id, module_id = module_id, module_name = module_name });
            }
            else
            {
                ViewBag.Message = message;
                ViewBag.Status = status;
                return View();
            }
        }

        public Boolean check_duplicate_items_factors(string item,int fn_id)
        {
            using (var context = new DBModels())
            {
                bool result = context.items_in_factor_table.Any(u => u.item == item && u.fn_id == fn_id);
                return result;
            }
        }

        //[HttpGet]
        /*public string calculate_total_factors(int id)
        {
            using (var context = new DBModels())
            {
             
                var data = (from a in context.factors_name_table
                            join b in context.items_in_factor_table
                            on a.fn_id equals b.fn_id
                            where a.module_id == id
                            group a by new { a.fn_id } into g
                            orderby g.Count() descending
                            select new
                            {
                                count = g.Count()
                            }).ToList();
                int cal = 1;
                foreach(var item in data)
                {
                    cal *= item.count;
                    //Debug.WriteLine(cal);
                }

                return cal+"";  
            }
        }*/
    }
}