using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using GameStore.Domain.Entities;
using Moq;
using GameStore.Domain.Abstract;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.Models;
using System.Web.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Web.WebPages;
using System.Web.Mvc.Routing.Constraints;
using System.Reflection.Emit;


/*
C:\Program Files\IIS Express>iisexpress.exe /path:..\GameStore.WebUI" /port:"5555"
*/
namespace GameStore.UnitTests {
    //-----------------------------------------------------------------------------------------------------//
    [TestClass]
    public class ISystemTesting {
        //*************************************************************************************///
        public FirefoxDriver drvr = null;
        public bool          isAUTORIZ     = false;
        public string        index_page    = "http://localhost:5555/";
        public string        cart_page     = "http://localhost:5555/Cart/Index";
        public string        url_aut       = "http://localhost:5555/Admin/index";
        public string        url_new_unit  = "http://localhost:5555/Admin/Create";
        public string        url_edit_unit = "http://localhost:5555/Admin/Edit?GameId=1";
        //*************************************************************************************///
        public virtual void Construct () {
            drvr = new FirefoxDriver();
            drvr.Url = url_aut;
            Thread.Sleep( 10 );
            drvr.FindElement( By.Id( "UserName" ) ).SendKeys( "admin" );
            drvr.FindElement( By.Id( "Password" ) ).SendKeys( "12345" + Keys.Enter );
            Thread.Sleep( 60 );
        }
        public virtual void Destruct () { drvr.Close(); }
        //*************************************************************************************///
        public virtual void StartTest () { }
        //*************************************************************************************///
    }
    //-----------------------------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------------------------//
    [TestClass]
    public class SystemTestingAUTH : ISystemTesting {
        //*************************************************************************************///
        public void Auth_Admin () {
            drvr.Url = url_aut;
            Thread.Sleep( 10 );
            string res = drvr.FindElement( By.XPath( "/html/body/div[1]/div/title_test" ) ).Text;
            if( res == "ADMIN_PANEL" ) isAUTORIZ = true;
            Assert.AreEqual( "ADMIN_PANEL", res );
        }
        //*************************************************************************************///
        [TestMethod]
        public override void StartTest () {
            Construct();
            Auth_Admin();
            Destruct();
        }
        //*************************************************************************************///
    }
    //-----------------------------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------------------------//
    [TestClass]
    public class SystemTestingAdminCRUDUnit : ISystemTesting {
        //*************************************************************************************///
        public void Add_New_Unit_Admin () {
            drvr.Url = url_new_unit;
            Thread.Sleep( 10 );
            drvr.FindElement( By.Id( "Name" ) ).SendKeys( "TOVAR 1" );
            drvr.FindElement( By.Id( "Description" ) ).SendKeys( "SUPER TOVAR 1" );
            drvr.FindElement( By.Id( "Category" ) ).SendKeys( "PC 1" );
            drvr.FindElement( By.Id( "Price" ) ).SendKeys( "2300" + Keys.Enter );
            Thread.Sleep( 5 );
            string res = drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]" ) ).Text;
            if( !res.IsEmpty() ) res = "add";
            Assert.AreEqual( "add", res );
        }
        //*************************************************************************************///
        public void Edit_Unit_Admin () {
            drvr.Url = url_edit_unit;
            Thread.Sleep( 10 );
            drvr.FindElement( By.Id( "Name" ) ).SendKeys( "TOVAR EDITED" );
            drvr.FindElement( By.Id( "Description" ) ).SendKeys( "SUPER TOVAR EDITED" );
            drvr.FindElement( By.Id( "Category" ) ).SendKeys( "PC EDITED" );
            drvr.FindElement( By.Id( "Price" ) ).SendKeys( "2300" + Keys.Enter );
            Thread.Sleep( 5 );
            string res = drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]" ) ).Text;
            if( !res.IsEmpty() ) res = "edited";
            Assert.AreEqual( "edited", res );
        }

        public void Delete_Unit_Admin () {
            drvr.Url = url_aut;
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[1]/div/div[2]/table/tbody/tr[12]/td[4]/form/input[2]" ) ).Click();
                              

            Thread.Sleep( 5 );
            string res = drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]" ) ).Text;
            if( !res.IsEmpty() ) res = "delete";
            Assert.AreEqual( "delete", res );
        }
        //*************************************************************************************/// 
        [TestMethod]
        public override void StartTest () {
            Construct();
            Add_New_Unit_Admin();
            Destruct();

            Construct();
            Edit_Unit_Admin();
            Destruct();

            Construct();
            Delete_Unit_Admin();
            Destruct();
        }
        //*************************************************************************************///
    }
    //-----------------------------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------------------------//
    [TestClass]
    public class SystemTestingCard : ISystemTesting {
        //*************************************************************************************///
        public bool Add_To_Card_Unit () {
            drvr.Url = index_page;
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div[1]/form/div/input[3]" ) ).Click();
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div/a[1]" ) ).Click();// продолжить покупки
            Thread.Sleep( 10 );
            bool r =false;
            drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]/a" ) ).Click();
            string res =   drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/table/tbody/tr/td[1]" ) ).Text;
            if( !res.IsEmpty() ) { res = "add"; r = true; }
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/table/tbody/tr/td[5]/form/input[3]" ) ).Click();
            Assert.AreEqual( "add", res );
            return r;
        }
        //*************************************************************************************///
        public bool Add_To_Card_Unit_Delete_See_Empty () {
            //добавление товаров в корзину + удаление всех товаров + просмотр что корзина пустая
            drvr.Url = index_page;
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div[1]/form/div/input[3]" ) ).Click();
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/table" +
               "/tbody/tr/td[5]/form/input[3]" ) ).Click();

            string res = drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/table/tfoot/tr/td[1]" ) ).Text;

            bool r = false;
            if( !res.IsEmpty() ) { res = "Итого:"; r = true; }
            Assert.AreEqual( "Итого:", res );
            return r;
        }
        //*************************************************************************************///
        public bool Add_To_Card_Unit_Goto_Processing () {
            //добавление товаров в корзину и переход для оформления
            drvr.Url = index_page;
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div[2]/form/div/input[3]" ) ).Click();
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div/a[2]" ) ).Click();
            Thread.Sleep( 10 );
            string res = drvr.FindElement(By.XPath("/html/body/div[3]/div[2]/h2")).Text;
            bool r = false;
            if( !res.IsEmpty() ) { res = "processing"; r = true; }
            drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]/a" ) ).Click();
            Thread.Sleep( 5 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/table/tbody/tr/td[5]/form/input[3]" ) ).Click();
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[1]/div/a[1]" ) ).Click();
            Assert.AreEqual( "processing", res );
            return r;
        }
        //*************************************************************************************///
        public bool Add_To_Card_Unit_Goto_Processing_Ordered_With_Bad_Data () {
            //добавление товаров в корзину + переход для оформления  + оформление с bad логами
            drvr.Url = index_page;
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div[2]/form/div/input[3]" ) ).Click();
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div/a[2]" ) ).Click();
            Thread.Sleep( 10 );
            string res = drvr.FindElement(By.XPath("/html/body/div[3]/div[2]/h2")).Text;

            if( !res.IsEmpty() ) {
                Dictionary< string, bool > error_list = new  Dictionary< string, bool >();

                /*
                    Укажите как вас зовут              /html/body/div[3]/div[2]/form/div[1]/ul/li[1]
                    Вставьте первый адрес доставки     /html/body/div[3]/div[2]/form/div[1]/ul/li[2]
                    Укажите город                      /html/body/div[3]/div[2]/form/div[1]/ul/li[3]
                    Укажите страну                     /html/body/div[3]/div[2]/form/div[1]/ul/li[4]
                */
                string er;
                drvr.FindElement( By.XPath( "//*[@id='Name']" ) ).SendKeys( "" );
                drvr.FindElement( By.XPath( "//*[@id='Line1']" ) ).SendKeys( "Kremlin 21" );
                drvr.FindElement( By.XPath( "//*[@id='City']" ) ).SendKeys( "Moscow" );
                drvr.FindElement( By.XPath( "//*[@id='Country']" ) ).SendKeys( "Russia" );
                drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/form/div[9]/input" ) ).Click();
                Thread.Sleep( 5 );
                er = drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/form/div[1]/ul/li[1]" ) ).Text;
                if( !er.IsEmpty() ) error_list.Add( "name", true );
                else error_list.Add( "name", false );
                drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]/a" ) ).Click();
                Thread.Sleep( 5 );
                drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div/a[2]" ) ).Click();
                Thread.Sleep( 5 );

                drvr.FindElement( By.XPath( "//*[@id='Name']" ) ).SendKeys( "Ivavon Ivan" );
                drvr.FindElement( By.XPath( "//*[@id='Line1']" ) ).SendKeys( "" );
                drvr.FindElement( By.XPath( "//*[@id='City']" ) ).SendKeys( "Moscow" );
                drvr.FindElement( By.XPath( "//*[@id='Country']" ) ).SendKeys( "Russia" );
                drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/form/div[9]/input" ) ).Click();
                Thread.Sleep( 5 );
                er = drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/form/div[1]/ul/li" ) ).Text;
                if( !er.IsEmpty() ) error_list.Add( "address", true );
                else error_list.Add( "address", false );
                drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]/a" ) ).Click();
                Thread.Sleep( 5 );
                drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div/a[2]" ) ).Click();
                Thread.Sleep( 5 );

                drvr.FindElement( By.XPath( "//*[@id='Name']" ) ).SendKeys( "Ivavon Ivan" );
                drvr.FindElement( By.XPath( "//*[@id='Line1']" ) ).SendKeys( "Kremlin 21" );
                drvr.FindElement( By.XPath( "//*[@id='City']" ) ).SendKeys( "" );
                drvr.FindElement( By.XPath( "//*[@id='Country']" ) ).SendKeys( "Russia" );
                drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/form/div[9]/input" ) ).Click();
                Thread.Sleep( 5 );
                er = drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/form/div[1]/ul/li" ) ).Text;
                if( !er.IsEmpty() ) error_list.Add( "city", true );
                else error_list.Add( "city", false );
                drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]/a" ) ).Click();
                Thread.Sleep( 5 );
                drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div/a[2]" ) ).Click();
                Thread.Sleep( 5 );

                drvr.FindElement( By.XPath( "//*[@id='Name']" ) ).SendKeys( "Ivavon Ivan" );
                drvr.FindElement( By.XPath( "//*[@id='Line1']" ) ).SendKeys( "Kremlin 21" );
                drvr.FindElement( By.XPath( "//*[@id='City']" ) ).SendKeys( "Moscow" );
                drvr.FindElement( By.XPath( "//*[@id='Country']" ) ).SendKeys( "" );
                drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/form/div[9]/input" ) ).Click();
                Thread.Sleep( 5 );
                er = drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/form/div[1]/ul/li" ) ).Text;
                if( !er.IsEmpty() ) error_list.Add( "country", true );
                else error_list.Add( "country", false );
                drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]/a" ) ).Click();

                drvr.FindElement( By.XPath( "/html/body/div[1]/div[1]/a" ) ).Click();
                drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/table/tbody/tr/td[5]/form/input[3]" ) ).Click();
                Thread.Sleep( 10 );
                bool total_err = true;
                foreach( var e in error_list ) { if( !e.Value ) { total_err = false; break; } }

                Assert.AreEqual( true, total_err );
                return total_err;
            }
            return false;
        }
        //*************************************************************************************///
        public bool Add_To_Card_Unit_Goto_Processing_Ordered_With_Good_Data () {
            //добавление товаров в корзину + переход для оформления  + оформление с good логами
            drvr.Url = index_page;
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div[2]/form/div/input[3]" ) ).Click();
            Thread.Sleep( 10 );
            drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/div/a[2]" ) ).Click();
            Thread.Sleep( 10 );
            string res = drvr.FindElement(By.XPath("/html/body/div[3]/div[2]/h2")).Text;

            if( !res.IsEmpty() ) {

                Dictionary< string, string > error_list = new  Dictionary< string, string >();
                drvr.FindElement( By.XPath( "//*[@id='Name']" ) ).SendKeys( "Ivavon Ivan" );
                drvr.FindElement( By.XPath( "//*[@id='Line1']" ) ).SendKeys( "Kremlin 21" );
                drvr.FindElement( By.XPath( "//*[@id='City']" ) ).SendKeys( "Moscow" );
                drvr.FindElement( By.XPath( "//*[@id='Country']" ) ).SendKeys( "Russia" );
                drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/form/div[9]/input" ) ).Click();

                bool total_err = false;
                string resuls = drvr.FindElement( By.XPath( "/html/body/div[3]/div[2]/h2" ) ).Text;
                if( !resuls.IsEmpty() ) { resuls = "Спасибо!"; total_err = true; }
                Assert.AreEqual( "Спасибо!", resuls );
                return total_err;
            }
            return false;
        }
        //*************************************************************************************///

        [TestMethod]
        public override void StartTest () {
            List< bool > err = new List< bool >();
            drvr = new FirefoxDriver();
            err.Add( Add_To_Card_Unit() );
            Destruct();

            drvr = new FirefoxDriver();
            err.Add( Add_To_Card_Unit_Delete_See_Empty() );
            Destruct();

            drvr = new FirefoxDriver();
            err.Add( Add_To_Card_Unit_Goto_Processing() );
            Destruct();


            drvr = new FirefoxDriver();
            err.Add( Add_To_Card_Unit_Goto_Processing_Ordered_With_Bad_Data() );
            Destruct();

            drvr = new FirefoxDriver();
            err.Add( Add_To_Card_Unit_Goto_Processing_Ordered_With_Good_Data() );
            Destruct();

            bool total_err = true;
            foreach( var e in err ) { if( !e ) { total_err = false; break; } }
            Assert.AreEqual( true, total_err );
        }
        //*************************************************************************************///
    }
    //-----------------------------------------------------------------------------------------------------//

}
