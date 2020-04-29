using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
namespace door_ver2_1
{
    public class selenium //싱글톤 패턴사용
    {
        //다중 스레드를 사용하여도 안전한 코드
        private static readonly Lazy<selenium> sn = new Lazy<selenium>(() => new selenium());
        //Lazy 사용되기 전까지 생성을 지연시키는코드
        //스레드에 안전하다.

        IWebDriver dr = null;
        CookieContainer cookie = new CookieContainer();
        public static selenium Instance
        {
            get
            {
                return sn.Value;
            }
        }
        public string login_sn(string url,string id, string pwd)//셀레니움을 이용하여 로그인 후 쿠키를 가져오게 할것이다.
        {
            string name = String.Empty;
            using (IWebDriver drs = new ChromeDriver())
            {
                drs.Url = url;//"https://door.deu.ac.kr/sso/login.aspx";
                drs.Manage().Window.Minimize();

                if(url.Contains("deu"))
                {
                    IWebElement ele = drs.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/input"));
                    ele.Clear();
                    ele.SendKeys(id);
                    ele = drs.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[2]/td/input"));
                    ele.Clear();
                    ele.SendKeys(pwd);
                    drs.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[3]/a")).Click();
                    name = drs.FindElement(By.XPath("/html/body/div[2]/div[1]/div/div[1]/a[1]")).Text;
                }
                else if(url.Contains("tu"))
                {
                    IWebElement ele = drs.FindElement(By.XPath("/html/body/main/div/div[2]/div/form/input[1]"));
                    ele.Clear();
                    ele.SendKeys(id);
                    ele = drs.FindElement(By.XPath("/html/body/main/div/div[2]/div/form/input[2]"));
                    ele.Clear();
                    ele.SendKeys(pwd);
                    drs.FindElement(By.XPath("/html/body/main/div/div[2]/div/form/div/a")).Click();
                    name = drs.FindElement(By.XPath("/html/body/header/nav/div/div[1]/div[3]/div[2]/div/button/span")).Text;
                }
                
                if (isAlertPresent(drs))
                {
                    return "로그인 실패";
                }
                foreach (var s in drs.Manage().Cookies.AllCookies)
                {
                    System.Net.Cookie gs = new System.Net.Cookie();
                    string[] f = s.ToString().Split(';');
                    string[] g = f[0].Split('=');
                    gs.Name = g[0];
                    gs.Value = g[1];
                    
                    if(url.Contains("deu"))
                    {
                        this.cookie.Add(new Uri("https://door.deu.ac.kr"), gs);
                    }
                    if (url.Contains("tu"))
                    {
                        this.cookie.Add(new Uri("https://class.deu.ac.kr"), gs);
                    }
                    
                }

                return name;
            }

        }
        public CookieContainer getCooke()
        {
            return this.cookie;
        }
        public bool isAlertPresent(IWebDriver dr)
        {
            try
            {
                dr.SwitchTo().Alert().Accept();
                return true;
            }   // try 
            catch (NoAlertPresentException Ex)
            {
                return false;
            }   // catch 
        }   // isAlertPresent()
        public void link_mv(string target_link, string[,] sum_link, int arr_first, int arr_sec,string[] info)
        {
            if (dr != null)
            {
                try
                {
                    dr.Url = target_link + sum_link[arr_first, arr_sec];
                }
                catch
                {
                    dr = new ChromeDriver();

                    dr.Url = "https://door.deu.ac.kr/sso/login.aspx";
                    IWebElement ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/input"));
                    ele.Clear();
                    ele.SendKeys(info[0]);
                    ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[2]/td/input"));
                    ele.Clear();
                    ele.SendKeys(info[1]);
                    dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[3]/a")).Click();
                    dr.Url = target_link + sum_link[arr_first, arr_sec];
                }
                //driverService.HideCommandPromptWindow = true;
            }
            else
            {
                dr = new ChromeDriver();

                dr.Url = "https://door.deu.ac.kr/sso/login.aspx";
                IWebElement ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/input"));
                ele.Clear();
                ele.SendKeys(info[0]);
                ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[2]/td/input"));
                ele.Clear();
                ele.SendKeys(info[1]);
                dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[3]/a")).Click();
                dr.Url = target_link + sum_link[arr_first, arr_sec];
                //driverService.HideCommandPromptWindow = true;
                //if (dr != null)
                //{
                //    dr.Url = lectureDoweek + room_code[cB_site.SelectedIndex, 0];
                //    driverService.HideCommandPromptWindow = true;
                //}
                //else
                //{
                //    dr = new ChromeDriver();

                //    dr.Url = "https://door.deu.ac.kr/sso/login.aspx";
                //    IWebElement ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/input"));
                //    ele.Clear();
                //    ele.SendKeys(id);
                //    ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[2]/td/input"));
                //    ele.Clear();
                //    ele.SendKeys(pwd);
                //    dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[3]/a")).Click();
                //    dr.Url = lectureRoom_door + room_code[cB_site.SelectedIndex, 0];
                //    driverService.HideCommandPromptWindow = true;

                //}
            }
        }
    }
}
