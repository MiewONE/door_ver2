using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.IO;
using URLEncoding_SpecialChar2unicode;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using HtmlAgilityPack;
namespace door_ver2_1
{
    public partial class MiewOne_door : Form
    {
        string name = null;
        string lectureRoom = "http://door.deu.ac.kr/LMS/LectureRoom/CourseHomeworkStudentList/",
            lectureRoom_main = "http://door.deu.ac.kr/LMS/LectureRoom/Main/",
            lectureRoom_door = "http://door.deu.ac.kr/LMS/LectureRoom/CourseLectureInfo/",
            lectureDoweek = "http://door.deu.ac.kr/LMS/LectureRoom/DoorWeeks/";
        //
        ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
        private CookieContainer cookiecontainer = new CookieContainer();
        IWebDriver dr = null;
        string[,] room_code = new string[20, 2];
        Point moveForm = new Point(0, 0);
        int c_cnt;
        string id = "",
                pwd = "";
        string userid;
        private BackgroundWorker brk;
        string[,] door_ck;

        public MiewOne_door()
        {
            InitializeComponent();
            c_cnt = 0;
            pl_top_image.Visible = false;
            brk = new BackgroundWorker();
            brk.WorkerReportsProgress = true;
            brk.WorkerSupportsCancellation = true;
            brk.WorkerSupportsCancellation = true;
            brk.WorkerSupportsCancellation = true;
            brk.DoWork += new DoWorkEventHandler(f_brk);
            brk.ProgressChanged += new ProgressChangedEventHandler(f_brk_processing);
            brk.RunWorkerCompleted += new RunWorkerCompletedEventHandler(f_brk_complete);
            userid = id;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pl_top_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                moveForm = new Point(-e.X, -e.Y);
            }
        }

        private void pl_top_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(
                    this.Location.X + (moveForm.X + e.X),
                    this.Location.Y + (moveForm.Y + e.Y)
                    );
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbx_id.Enabled = false;
            if (tbx_id.Text != userid)
            {
                MessageBox.Show("너 누구야", "Warring");
                tbx_id.Enabled = true;
                //Close();
            }
            else
            {
                pl_top_image.Visible = true;
                button1.Enabled = false;
                brk.RunWorkerAsync();
            }


            //dGV_report.FirstDisplayedScrollingRowIndex = dGV_report.Rows.Count - 1;
        }
        private void login()
        {

            string domstr = String.Empty;
            //_invoke(this, this.TopMost, true);
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    this.TopMost = true;
                }));
            }
            using (IWebDriver dr = new ChromeDriver())
            {

                dr.Url = "https://door.deu.ac.kr/sso/login.aspx";
                dr.Manage().Window.Minimize();
                driverService.HideCommandPromptWindow = true;

                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        this.TopMost = false;
                    }));
                }
                //_invoke(this, this.TopMost, false);
                IWebElement ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/input"));
                ele.Clear();
                ele.SendKeys(id);
                ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[2]/td/input"));
                ele.Clear();
                ele.SendKeys(pwd);
                dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[3]/a")).Click();
                name = dr.FindElement(By.XPath("/html/body/div[2]/div[1]/div/div[1]/a[1]")).Text;
                //_invoke(lb_hello, lb_hello.Text, $"안녕하세요 {name}씨");
                if(lb_hello.InvokeRequired)
                {
                    lb_hello.Invoke(new MethodInvoker(delegate
                    {
                        lb_hello.Text = $"안녕하세요 {name}씨";
                    }));
                }
                else
                {
                    lb_hello.Text = $"안녕하세요 {name}씨";
                }
                foreach (var s in dr.Manage().Cookies.AllCookies)
                {
                    System.Net.Cookie gs = new System.Net.Cookie();
                    string[] f = s.ToString().Split(';');
                    string[] g = f[0].Split('=');
                    gs.Name = g[0];
                    gs.Value = g[1];
                    this.cookiecontainer.Add(new Uri("https://door.deu.ac.kr"), gs);
                }
            }

            #region 세션 가져오기

            domstr = getSession(cookiecontainer, "http://door.deu.ac.kr/MyPage", "http://door.deu.ac.kr/");

            string[] _class = domstr.Split('<'); // 52 + a(12)
            string[] code_class = new string[20];
            for (int i = 0, j = 0, k = 0; i < _class.Length; i++)
            {
                if (_class[i].Contains("javascript:goRoom"))
                {
                    code_class[j] = _class[i];//과목*2 넘어가면안돼
                    //room_code[j, 0] = code_class[j].Substring(code_class[j].IndexOf('(') + 2, 5);
                    string[] cut = code_class[j].Split('>');
                    if (cut[1].Length > 2)
                    {
                        room_code[k, 0] = cut[0].Substring(cut[0].IndexOf('\'') + 1, 5);
                        room_code[k, 1] = cut[1];
                        //_invoke(cB_site, cB_site.Items.Add(room_code[k, 1]),null);
                        if (cB_site.InvokeRequired)
                        {
                            cB_site.Invoke(new MethodInvoker(delegate ()
                            {
                                cB_site.Items.Add(room_code[k, 1]);
                            }));
                        }
                        else
                        {
                            cB_site.Items.Add(room_code[k, 1]);
                        }

                        k++;
                    }
                    j++;
                }
            }
            #region 과제들고오기
            for (int i = 0; i < room_code.Length / 2; i++)
            {
                if (room_code[i, 0] != null)
                {
                    domstr = getSession(cookiecontainer, lectureRoom + room_code[i, 0], lectureRoom_main + room_code[i, 0]);
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(domstr);
                    //string xpath = "/html/body/div[2]/div[2]/div[5]/form/div/div/table/tbody/tr[2]/td[4]";

                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes($"/html/body/div[2]/div[2]/div[5]/form/div/div/table/tbody/tr[*]/td[2]");
                    if (nodes == null)
                    {
                        break;
                    }
                    string[,] vs = new string[nodes.Count, 7];
                    for (int j = 2; j <= nodes.Count + 1; j++)
                    {
                        for (int k = 1; k <= 5; k++)
                        {
                            HtmlNode cnt = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div[2]/div[5]/form/div/div/table/tbody/tr[{j}]/td[{5}]");
                            if (cnt.InnerText.Contains("미제출"))
                            {
                                cnt = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div[2]/div[5]/form/div/div/table/tbody/tr[{j}]/td[{k}]");
                                //vs1.Append(cnt.InnerText);
                                vs[j - 2, k] = cnt.InnerText;
                                //dGV_report.Rows.Add(vs);

                            }
                        }
                        if (vs[j - 2, 1] != null)
                        {
                            if (dGV_report.InvokeRequired)
                            {
                                dGV_report.Invoke(new MethodInvoker(delegate
                                {
                                    dGV_report.Rows.Add(room_code[i, 1], vs[j - 2, 1], vs[j - 2, 2], vs[j - 2, 3], vs[j - 2, 4], vs[j - 2, 5]);

                                }));
                            }
                            else
                            {
                                dGV_report.Rows.Add(vs);

                            }
                        }

                    }

                }


            }
            #endregion
            for (int i = 0; i < room_code.Length; i++)//door 조회
            {
                domstr = getSession(cookiecontainer, lectureRoom_door + room_code[i, 0], lectureRoom_main + room_code[i, 0]);
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(domstr);

                HtmlNodeCollection door_nodes = doc.DocumentNode.SelectNodes("/html/body/div[2]/div[2]/div[5]/div[11]/table/tbody/tr[*]/td[3]");
                if (door_nodes == null)
                {
                    break;
                }
                //string[,] vs = new string[nodes.Count, 7];
                door_ck = new string[door_nodes.Count + 10, 6];

                for (int j = 2; j <= door_nodes.Count; j++)
                {
                    for (int k = 1; k <= 5; k++)
                    {
                        HtmlNode cnt = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div[2]/div[5]/div[11]/table/tbody/tr[{j}]/td[4]");

                        if (cnt.InnerText == "X")
                        {
                            cnt = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div[2]/div[5]/div[11]/table/tbody/tr[{j}]/td[{k}]");
                            door_ck[j - 2, k - 1] = cnt.InnerText;
                        }
                    }
                    if (door_ck[j - 2, 1] != null)
                    {
                        if (dGV_door.InvokeRequired)
                        {
                            dGV_door.Invoke(new MethodInvoker(delegate
                            {
                                dGV_door.Rows.Add(room_code[i, 1], door_ck[j - 2, 0], door_ck[j - 2, 1], door_ck[j - 2, 2], door_ck[j - 2, 3], door_ck[j - 2, 4], door_ck[j - 2, 5]);

                            }));
                        }
                        else
                        {
                            dGV_door.Rows.Add(door_ck);

                        }
                    }


                }
                



            }


        }
        private string getSession(CookieContainer cookiecontainer, string url, string refer)
        {
            string strSessionInfo = String.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            WebHeaderCollection webHeader = req.Headers;

            req.Accept = "*/*";
            req.ProtocolVersion = HttpVersion.Version11;
            req.KeepAlive = true;
            req.ContentType = "application/x-www-form-urlencoded";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36";
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            req.Host = "door.deu.ac.kr";
            req.Method = WebRequestMethods.Http.Get;
            req.Referer = refer;
            req.CookieContainer = this.cookiecontainer;
            //쿠키 정보 출력
            //if (req.CookieContainer.Count > 0)
            //{
            //    foreach (System.Net.Cookie cook in req.CookieContainer.GetCookies(new Uri(url)))
            //    {
            //        string c = String.Empty;
            //        c += "Cookie:\n" + $"{cook.Name} = {cook.Value}\n" + $"Domain: {cook.Domain}\n" + $"Path: {cook.Path}\n" +
            //            $"Secure: {cook.Secure}\n" +
            //            $"When issued: {cook.TimeStamp}\n" +
            //            $"Expires: {cook.Expires} (expired? {cook.Expired})\n" +
            //            $"Don't save: {cook.Discard}\n" +
            //            $"Comment: {cook.Comment}\n" +
            //            $"Uri for comments: {cook.CommentUri}\n" +
            //            $"Version: RFC {(cook.Version == 1 ? 2109 : 2965)}" +
            //            $"String: {cook}";
            //        MessageBox.Show(c);
            //    }
            //}
            string Info = String.Empty;

            #endregion


            try
            {
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {


                    StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    string strResult = sr.ReadToEnd();
                    Console.WriteLine(res.ResponseUri);

                    try
                    {
                        strSessionInfo = strResult;
                    }
                    finally
                    {
                        if (sr != null)
                            sr.Close();
                        //res.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
                MessageBox.Show("서버 요청이 실패하였습니다.");
                Close();
            }

            strSessionInfo = strSessionInfo.Replace("\r\n", "");
            strSessionInfo = strSessionInfo.Replace("\t", "");

            return strSessionInfo;
        }

        private void cB_site_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (button2.Text == "과제")
            {
                if (cB_site.SelectedIndex >= 0)
                {
                    //System.Diagnostics.Process.Start(lectureRoom + room_code[cB_site.SelectedIndex,0]);
                    if (dr != null)
                    {
                        dr.Url = lectureRoom + room_code[cB_site.SelectedIndex, 0];
                        driverService.HideCommandPromptWindow = true;
                    }
                    else
                    {
                        dr = new ChromeDriver();

                        dr.Url = "https://door.deu.ac.kr/sso/login.aspx";
                        IWebElement ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/input"));
                        ele.Clear();
                        ele.SendKeys(id);
                        ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[2]/td/input"));
                        ele.Clear();
                        ele.SendKeys(pwd);
                        dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[3]/a")).Click();
                        dr.Url = lectureRoom + room_code[cB_site.SelectedIndex, 0];
                        driverService.HideCommandPromptWindow = true;

                    }

                }
            }
            else
            {
                if (cB_site.SelectedIndex >= 0)
                {
                    if (dr != null)
                    {
                        dr.Url = lectureDoweek + room_code[cB_site.SelectedIndex, 0];
                        driverService.HideCommandPromptWindow = true;
                    }
                    else
                    {
                        dr = new ChromeDriver();

                        dr.Url = "https://door.deu.ac.kr/sso/login.aspx";
                        IWebElement ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/input"));
                        ele.Clear();
                        ele.SendKeys(id);
                        ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[2]/td/input"));
                        ele.Clear();
                        ele.SendKeys(pwd);
                        dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[3]/a")).Click();
                        dr.Url = lectureRoom_door + room_code[cB_site.SelectedIndex, 0];
                        driverService.HideCommandPromptWindow = true;

                    }

                }

            }
        }
        #region brk
        public void f_brk(object sender, DoWorkEventArgs e)
        {
            login();
        }

        private void tbx_id_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                if (tbx_id.Text != userid)
                {
                    MessageBox.Show("너 누구야", "Warring");
                    //Close();
                }
                else
                {
                    pl_top_image.Visible = true;
                    button1.Enabled = false;
                    brk.RunWorkerAsync();
                }
            }
        }


        private void dGV_report_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex<0)
            {
                return;
            }
            string[] test = new string[10];
            for (int i = 0; i < test.Length; i++)
            {
                if (room_code[i, 1] == null || room_code[i, 0] == null)
                {
                    break;
                }
                else
                {
                    test[i] = room_code[i, 1];
                }
            }
            for (int i = 0; i < test.Length; i++)
            {

                if (dGV_report.Rows[e.RowIndex].Cells[0].Value.ToString() == test[i])
                {
                    IWebDriver dr = new ChromeDriver();

                    dr.Url = "https://door.deu.ac.kr/sso/login.aspx";
                    IWebElement ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/input"));
                    ele.Clear();
                    ele.SendKeys(id);
                    ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[2]/td/input"));
                    ele.Clear();
                    ele.SendKeys(pwd);
                    dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[3]/a")).Click();


                    dr.Url = lectureRoom + room_code[i, 0];


                    driverService.HideCommandPromptWindow = true;
                }
            }
        }

        private void cellclick(string para, DataGridViewCellEventArgs e)
        {

        }

        private void dGV_door_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            string[] test = new string[10];
            for (int i = 0; i < test.Length; i++)
            {
                if (room_code[i, 1] == null || room_code[i, 0] == null)
                {
                    break;
                }
                else
                {
                    test[i] = room_code[i, 1];
                }
            }
            for (int i = 0; i < test.Length; i++)
            {

                if (dGV_door.Rows[e.RowIndex].Cells[0].Value.ToString() == test[i])
                {
                    IWebDriver dr = new ChromeDriver();

                    dr.Url = "https://door.deu.ac.kr/sso/login.aspx";
                    IWebElement ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/input"));
                    ele.Clear();
                    ele.SendKeys(id);
                    ele = dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[2]/td/input"));
                    ele.Clear();
                    ele.SendKeys(pwd);
                    dr.FindElement(By.XPath("/html/body/form/div[2]/div[1]/div/table/tbody/tr[1]/td[3]/a")).Click();



                    dr.Url = lectureRoom_door + room_code[i, 0];


                    driverService.HideCommandPromptWindow = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "과제")
            {
                button2.Text = "Door";
            }
            else
            {
                button2.Text = "과제";
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            //this.WindowState = FormWindowState.Maximized;
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }



        public void f_brk_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                
                button1.Enabled = true;
                pl_top_image.Visible = false;
                lb_reportcnt.Text = "아이구야 " + dGV_door.RowCount.ToString() + "개나 안했네...";
                lb_doorcnt.Text =  "아이구야... " + dGV_report.RowCount.ToString() + "개나 해야해?";
                //_invoke(pl_top_image, pl_top_image.Visible, false);
                MessageBox.Show("작업이 완료되었습니다.");
            }

        }
        public void f_brk_processing(object sender, ProgressChangedEventArgs e)
        {
            //_invoke(pl_top_image, pl_top_image.Visible, true);
            
        }
        #endregion

        private void _invoke(Control crl, ContainerControl obj, string cmd)
        {
            if (crl.InvokeRequired)
            {
                crl.Invoke(new MethodInvoker(delegate
                {
                    obj.Text = cmd;
                }));
            }
            else
            {
                obj.Text = cmd;
            }
        }
        private void _invoke(Control crl, Object obj, bool cmd)
        {
            if (crl.InvokeRequired)
            {
                crl.Invoke(new MethodInvoker(delegate
                {
                    obj = cmd;
                }));
            }
            else
            {
                obj = cmd;
            }
        }
    }
}
