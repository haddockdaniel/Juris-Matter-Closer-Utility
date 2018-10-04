using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Globalization;
using Gizmox.Controls;
using JDataEngine;
using JurisAuthenticator;
using JurisUtilityBase.Properties;

namespace JurisUtilityBase
{
    public partial class UtilityBaseMain : Form
    {
        #region Private  members

        private JurisUtility _jurisUtility;

        #endregion

        #region Public properties

        public string CompanyCode { get; set; }

        public string JurisDbName { get; set; }

        public string JBillsDbName { get; set; }

        public int FldClient { get; set; }

        public int FldMatter { get; set; }
        public string singleTimekeeper = "";
        public string singlePracticeClass = "";
        public string singleClient = "";
        public string ExceptTimekeeper = "";        
        public string ExceptClient = "";
        #endregion

        #region Constructor

        public UtilityBaseMain()
        {
            InitializeComponent();
            _jurisUtility = new JurisUtility();
        }

        #endregion

        #region Public methods

        public void LoadCompanies()
        {
            var companies = _jurisUtility.Companies.Cast<object>().Cast<Instance>().ToList();
//            listBoxCompanies.SelectedIndexChanged -= listBoxCompanies_SelectedIndexChanged;
            listBoxCompanies.ValueMember = "Code";
            listBoxCompanies.DisplayMember = "Key";
            listBoxCompanies.DataSource = companies;
//            listBoxCompanies.SelectedIndexChanged += listBoxCompanies_SelectedIndexChanged;
            var defaultCompany = companies.FirstOrDefault(c => c.Default == Instance.JurisDefaultCompany.jdcJuris);
            if (companies.Count > 0)
            {
                listBoxCompanies.SelectedItem = defaultCompany ?? companies[0];
            }
        }

        #endregion

        #region MainForm events

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void listBoxCompanies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_jurisUtility.DbOpen)
            {
                _jurisUtility.CloseDatabase();
            }
            CompanyCode = "Company" + listBoxCompanies.SelectedValue;
            _jurisUtility.SetInstance(CompanyCode);
            JurisDbName = _jurisUtility.Company.DatabaseName;
            JBillsDbName = "JBills" + _jurisUtility.Company.Code;
            _jurisUtility.OpenDatabase();
          
            cbDays.Visible = true;
            dtSince.Visible = false;
            rbDays.Checked = true;

            string TkprIndex2;
            cbBT.ClearItems();
            string SQLTkpr2 = "select employee from (select '* (All Timekeepers)' as employee union all select case when empinitials<='   ' then empid else empinitials end + case when len(empinitials)=1 then '     ' when len(empinitials)=2 then '     ' when len(empinitials)=3 then '   ' else '  ' end +  empname as employee from employee where empvalidastkpr='Y') Emp order by employee";
            DataSet myRSTkpr2 = _jurisUtility.RecordsetFromSQL(SQLTkpr2);

            if (myRSTkpr2.Tables[0].Rows.Count == 0)
                cbBT.SelectedIndex = 0;
            else
            {
                foreach (DataRow dr in myRSTkpr2.Tables[0].Rows)
                {
                    TkprIndex2 = dr["employee"].ToString();
                    cbBT.Items.Add(TkprIndex2);
                }
            }

            string CliIndex2;
            cbClient.ClearItems();
            string SQLCli2 = "select Client from (select '* (All Clients)' as Client union all select dbo.jfn_formatclientcode(clicode) + '   ' +  clireportingname as Client from Client) Emp order by Client";
            DataSet myRSCli2 = _jurisUtility.RecordsetFromSQL(SQLCli2);

            if (myRSCli2.Tables[0].Rows.Count == 0)
                cbClient.SelectedIndex = 0;
            else
            {
                foreach (DataRow dr in myRSCli2.Tables[0].Rows)
                {
                    CliIndex2 = dr["Client"].ToString();
                    cbClient.Items.Add(CliIndex2);
                }
            }

            string PCIndex2;
            cbPC.ClearItems();
            string SQLPC2 = "select PC from (select '* (All Practice Classes)' as PC union all select prctclscode + '   ' +  prctclsdesc as PC from PracticeClass) Emp order by PC";
            DataSet myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2.Tables[0].Rows.Count == 0)
                cbPC.SelectedIndex = 0;
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                {
                    PCIndex2 = dr["PC"].ToString();
                    cbPC.Items.Add(PCIndex2);
                }
            }



            string TkprIndex;
            cbExcBT.ClearItems();
            string SQLTkpr = "select employee from (select '* (None)' as employee union all select case when empinitials<='   'then empid else empinitials end + case when len(empinitials)=1 then '     ' when len(empinitials)=2 then '     ' when len(empinitials)=3 then '   ' else '  ' end +  empname as employee from employee where empvalidastkpr='Y') Emp order by employee";
            DataSet myRSTkpr = _jurisUtility.RecordsetFromSQL(SQLTkpr);

            if (myRSTkpr.Tables[0].Rows.Count == 0)
                cbExcBT.SelectedIndex = 0;
            else
            {
                foreach (DataTable table in myRSTkpr.Tables)
                {

                    foreach (DataRow dr in table.Rows)
                    {
                        TkprIndex = dr["employee"].ToString();
                        cbExcBT.Items.Add(TkprIndex);
                    }
                }

            }

            string CliIndex;
            cbExcCli.ClearItems();
            string SQLCli = "select Client from (select '* (None)' as Client union all select dbo.jfn_formatclientcode(clicode) + '   ' +  clireportingname as Client from Client) Emp order by Client";
            DataSet myRSCli = _jurisUtility.RecordsetFromSQL(SQLCli);

            if (myRSCli.Tables[0].Rows.Count == 0)
                cbExcCli.SelectedIndex = 0;
            else
            {
                foreach (DataTable table in myRSCli.Tables)
                {

                    foreach (DataRow dr in table.Rows)
                    {
                        CliIndex = dr["Client"].ToString();
                        cbExcCli.Items.Add(CliIndex);
                    }
                }

            }

            cbBT.SelectedIndex =0;
            cbClient.SelectedIndex = 0;
            cbPC.SelectedIndex = 0;
            cbExcBT.SelectedIndex = 0;
            cbExcCli.SelectedIndex = 0;
            cbDays.SelectedIndex = 0;




        }



        #endregion

        #region Private methods

        private void DoDaFix()
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            toolStripStatusLabel.Text = "Executing Matter Closer....";
            statusStrip.Refresh();
            UpdateStatus("Executing Matter Closer", 0, 5);

            string TBSql = @"Select   case when wip <> 0.00 or UnbilledTime <> 0 or UnbilledExp <> 0 or UnpostedTime <> 0 or UnpostedExp <> 0 or UnpostedVouchers <> 0 or OpenVouchers <> 0 or openbills <> 0 or AR <> 0.00 or PPD <> 0.00 or trust <> 0.00 then 'Unresolved - Review Exception Report' else 'Ready to Close' end as MatterStatus, dbo.jfn_formatclientcode(clicode) as Client, Clireportingname, dbo.jfn_formatmattercode(matcode) as Matter,MatPracticeClass as PC,
            Matreportingname, empinitials as BillTkpr, OrigTkpr, matsysnbr,convert(varchar(10),matdateopened,101) as OpenDate,
            case when matstatusflag='O' then 'Open' when matstatusflag='F' then 'Final Bill Sent' else matstatusflag end as Status, 
            case when convert(varchar(10), matdatelastwork,101)='01/01/1900' then 'Never' else convert(varchar(10), matdatelastwork,101) end  as LastWorked,
            case when  convert(varchar(10), matdatelastbill,101)='01/01/1900' then 'Never' else convert(varchar(10), matdatelastbill,101) end as LastBIlled,
            case when convert(varchar(10),  matdatelastexp,101)='01/01/1900' then 'Never' else convert(varchar(10),  matdatelastexp,101) end as LastExpense,
            case when convert(varchar(10), matdatelastpaymt,101)='01/01/1900' then 'Never' else convert(varchar(10), matdatelastpaymt,101) end as LastPayment, unbilledtime, unbilledexp, unpostedtime, unpostedexp, unpostedvouchers, openvouchers, openbills, wip, ar, ppd, trust,
   case when wip <> 0.00 or UnbilledTime <> 0 or UnbilledExp <> 0 or UnpostedTime <> 0 or UnpostedExp <> 0 or UnpostedVouchers <> 0 or OpenVouchers <> 0 or openbills <> 0 or AR <> 0.00 or PPD <> 0.00 or trust <> 0.00 then 1 else 0 end as Unresolved, 
   case when wip <> 0.00 or UnbilledTime <> 0 or UnbilledExp <> 0 or UnpostedTime <> 0 or UnpostedExp <> 0 or UnpostedVouchers <> 0 or OpenVouchers <> 0 or openbills <> 0 or AR <> 0.00 or PPD <> 0.00 or trust <> 0.00 then 0 else 1 end as ReadytoClose
            from matter
            inner join client on matclinbr=clisysnbr
            inner join billto on matbillto=billtosysnbr
            inner join employee on empsysnbr=billtobillingatty
            inner join (select morigmat, max(case when ot=1 then empinitials else '' end) + max(case when ot=2 then  ' ' +  empinitials else '' end) + 
            max(case when ot=3 then   ' ' +  empinitials else '' end) +  max(case when ot=4 then   ' ' +  empinitials else '' end) + max(case when ot=5 then   ' ' +  empinitials else '' end) as OrigTkpr
            from (select morigmat, empinitials, rank() over (Partition by morigmat order by empinitials) as OT
            from matorigatty inner join employee on morigatty=empsysnbr)MO
            group by morigmat)MO on matsysnbr=morigmat
            inner join (select matsysnbr as matsys, sum(unbilledtime) as UnbilledTime, sum(UnbilledExp) as UnbilledExp, sum(unpostedtime) as UnpostedTime, sum(unpostedexp) as UnpostedExp,
            sum(unpostedvouchers) as UnpostedVouchers, sum(openVouchers) as OpenVouchers, sum(openBills) as OpenBills, sum(wipbalance) as WIp, sum(arbalance) as AR, sum(ppdbalance) as PPD, sum(trustbalance) as trust
            from (select matsysnbr, 0 as UnbilledTime, 0 as UnbilledExp, 0 as UnpostedTime, 0 as UnpostedExp, 0 as UnpostedVouchers, 0 as OpenVouchers,0 as OpenBills, 0 as WIPBalance,0 as ARBalance, matppdbalance as PPDBalance, 0 as TrustBalance
            from matter
            union all select armmatter, 0, 0, 0, 0, 0, 0, count(armbillnbr) as OpenBills,  0 as WIp, sum(armbaldue) as ARBalance, 0 as PPD, 0 as Trust from armatalloc where armbaldue<>0 group by armmatter
            union all select utmatter, count(utid), 0, 0, 0, 0, 0, 0, sum(utamount), 0, 0, 0 from unbilledtime group by utmatter union all
            select uematter, 0, count(ueid), 0,0,0,0,0,sum(ueamount), 0, 0, 0 from unbilledexpense group by uematter
            union all  select tbdmatter, 0,0, count(tbdid),0,0,0,0,0,0,0,0 from timebatchdetail  where tbdposted='N' and tbdid not in (select tbdid from timeentrylink) group by tbdmatter
            union all  select mattersysnbr, 0,0, count(entryid),0,0,0,0,0,0,0,0 from timeentry where entrystatus<=6 group by mattersysnbr
            union all  select ebdmatter, 0,0, 0,count(ebdid),0,0,0,0,0,0,0 from expbatchdetail where ebdposted='N' and ebdid not in (select ebdid from ExpenseEntryLink) group by ebdmatter
            union all  select mattersysnbr, 0,0, 0,count(entryid),0,0,0,0,0,0,0 from expenseentry where entrystatus<=6  group by mattersysnbr
            union all  select vbmmatter, 0,0, 0,0,count(vbdid) as VchCount,0,0,0,0,0,0  from voucherbatchmatdist inner join voucherbatchdetail on vbdbatch=vbmbatch and vbdrecnbr=vbmrecnbr where vbdposted='N' group by vbmmatter
            union all  select vmmatter,0,0, 0,0,0, count(vmvoucher) as Vch,0,0,0,0,0 from voucher inner join vouchermatdist on vmvoucher=vchvouchernbr where vchstatus='O' and vmamount-vmamountpaid<>0 group by vmmatter 
            union all  select tamatter,0,0,0,0,0,0,0,0,0,0, sum(tabalance) from trustaccount group by tamatter) Mat group by matsysnbr)MatList on matsysnbr=matsys";
               
            TBSql = TBSql + @" where (matstatusflag='O' or matstatusflag='F') ";
            if (rbDays.Checked == true)
            {
                string DaySince = cbDays.Text;
                TBSql = TBSql + @" and DATEDIFF(d, Matter.MatDateLastWork, GETDATE())>" + DaySince.ToString() +
                        " and DATEDIFF(d, Matter.MatDateLastExp, GETDATE())>" + DaySince.ToString() +
                            "  and DATEDIFF(d, Matter.MatDateLastBill, GETDATE())>" + DaySince.ToString() +
                            "   and DATEDIFF(d, Matter.MatDateLastPaymt, GETDATE())>" + DaySince.ToString();
            }

            else
            {
                string dt = dtSince.Text;
                TBSql = TBSql + @" and (Matter.MatDateLastWork)<convert(datetime,'" + dt.ToString() + "',101) " +
                        " and (Matter.MatDateLastExp)<convert(datetime,'" + dt.ToString() + "',101) " +
                            "  and (Matter.MatDateLastBill)<convert(datetime,'" + dt.ToString() + "',101) " +
                            "   and (Matter.MatDateLastPaymt)<convert(datetime,'" + dt.ToString() + "',101) ";
            }

            TBSql = TBSql + getTimeKeepers() + getClients() + getPCs() + getdtException() + getbtException() + getCliException();
            TBSql = TBSql + @" order by case when wip <> 0.00 or UnbilledTime <> 0 or UnbilledExp <> 0 or UnpostedTime <> 0 or UnpostedExp <> 0 or UnpostedVouchers <> 0 or OpenVouchers <> 0 or openbills <> 0 or AR <> 0.00 or PPD <> 0.00 or trust <> 0.00 then 'Unresolved - Review Exception Report' else 'Ready to Close' end, Clicode, Matcode";
            DataSet exceptions = _jurisUtility.RecordsetFromSQL(TBSql);
            ReportDisplay rpds = new ReportDisplay(exceptions);
            rpds.ShowDialog();
            object sumExcept;
            object sumToClose;
            sumExcept = exceptions.Tables[0].Compute("sum(unresolved)", string.Empty);
            sumToClose = exceptions.Tables[0].Compute("sum(ReadytoClose)", string.Empty);
            string message = sumExcept.ToString() + " matters cannot be closed due to open transactions. These can be reviewed on the exceptions report. " + sumToClose.ToString() + " matters will be closed. Do you wish to continue?";
            string caption = "Matters to Close";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(this, message, caption, buttons,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                toolStripStatusLabel.Text = "Executing Matter Closer....";
                statusStrip.Refresh();
                UpdateStatus("Executing Matter Closer", 1, 5);
                string MatterSql = @"update matter set matstatusflag='C',matlockflag=3, matdateclosed=cast(getdate() as date) where matsysnbr in (select matsysnbr from matter
            inner join client on matclinbr=clisysnbr
            inner join billto on matbillto=billtosysnbr
            inner join employee on empsysnbr=billtobillingatty
            inner join (select morigmat, max(case when ot=1 then empinitials else '' end) + max(case when ot=2 then  ' ' +  empinitials else '' end) + 
            max(case when ot=3 then   ' ' +  empinitials else '' end) +  max(case when ot=4 then   ' ' +  empinitials else '' end) + max(case when ot=5 then   ' ' +  empinitials else '' end) as OrigTkpr
            from (select morigmat, empinitials, rank() over (Partition by morigmat order by empinitials) as OT
            from matorigatty inner join employee on morigatty=empsysnbr)MO
            group by morigmat)MO on matsysnbr=morigmat
            inner join (select matsysnbr as matsys, sum(unbilledtime) as UnbilledTime, sum(UnbilledExp) as UnbilledExp, sum(unpostedtime) as UnpostedTime, sum(unpostedexp) as UnpostedExp,
            sum(unpostedvouchers) as UnpostedVouchers, sum(openVouchers) as OpenVouchers, sum(openBills) as OpenBills, sum(wipbalance) as WIp, sum(arbalance) as AR, sum(ppdbalance) as PPD, sum(trustbalance) as trust
            from (select matsysnbr, 0 as UnbilledTime, 0 as UnbilledExp, 0 as UnpostedTime, 0 as UnpostedExp, 0 as UnpostedVouchers, 0 as OpenVouchers,0 as OpenBills, 0 as WIPBalance,0 as ARBalance, matppdbalance as PPDBalance, 0 as TrustBalance
            from matter
            union all select armmatter, 0, 0, 0, 0, 0, 0, count(armbillnbr) as OpenBills,  0 as WIp, sum(armbaldue) as ARBalance, 0 as PPD, 0 as Trust from armatalloc where armbaldue<>0 group by armmatter
            union all select utmatter, count(utid), 0, 0, 0, 0, 0, 0, sum(utamount), 0, 0, 0 from unbilledtime group by utmatter union all
            select uematter, 0, count(ueid), 0,0,0,0,0,sum(ueamount), 0, 0, 0 from unbilledexpense group by uematter
            union all  select tbdmatter, 0,0, count(tbdid),0,0,0,0,0,0,0,0 from timebatchdetail  where tbdposted='N' and tbdid not in (select tbdid from timeentrylink) group by tbdmatter
            union all  select mattersysnbr, 0,0, count(entryid),0,0,0,0,0,0,0,0 from timeentry where entrystatus<=6 group by mattersysnbr
            union all  select ebdmatter, 0,0, 0,count(ebdid),0,0,0,0,0,0,0 from expbatchdetail where ebdposted='N' and ebdid not in (select ebdid from ExpenseEntryLink) group by ebdmatter
            union all  select mattersysnbr, 0,0, 0,count(entryid),0,0,0,0,0,0,0 from expenseentry where entrystatus<=6  group by mattersysnbr
            union all  select vbmmatter, 0,0, 0,0,count(vbdid) as VchCount,0,0,0,0,0,0  from voucherbatchmatdist inner join voucherbatchdetail on vbdbatch=vbmbatch and vbdrecnbr=vbmrecnbr where vbdposted='N' group by vbmmatter
            union all  select vmmatter,0,0, 0,0,0, count(vmvoucher) as Vch,0,0,0,0,0 from voucher inner join vouchermatdist on vmvoucher=vchvouchernbr where vchstatus='O' and vmamount-vmamountpaid<>0 group by vmmatter 
            union all  select tamatter,0,0,0,0,0,0,0,0,0,0, sum(tabalance) from trustaccount group by tamatter) Mat group by matsysnbr)MatList on matsysnbr=matsys ";
;

                MatterSql = MatterSql + @" where (matstatusflag='O' or matstatusflag='F') and  wip= 0.00 and UnbilledTime= 0 and UnbilledExp= 0 and UnpostedTime= 0 and UnpostedExp= 0 and UnpostedVouchers= 0 and OpenVouchers= 0 and openbills= 0 and AR= 0.00 and PPD= 0.00 and trust = 0.00";
                if (rbDays.Checked == true)
                {
                    string DaySince = cbDays.Text;
                    MatterSql = MatterSql + @" and DATEDIFF(d, Matter.MatDateLastWork, GETDATE())>" + DaySince.ToString() +
                            " and DATEDIFF(d, Matter.MatDateLastExp, GETDATE())>" + DaySince.ToString() +
                                "  and DATEDIFF(d, Matter.MatDateLastBill, GETDATE())>" + DaySince.ToString() +
                                "   and DATEDIFF(d, Matter.MatDateLastPaymt, GETDATE())>" + DaySince.ToString();
                }

                else
                {
                    string dt = dtSince.Text;
                    MatterSql = MatterSql + @" and (Matter.MatDateLastWork)<convert(datetime,'" + dt.ToString() + "',101) " +
                            " and (Matter.MatDateLastExp)<convert(datetime,'" + dt.ToString() + "',101) " +
                                "  and (Matter.MatDateLastBill)<convert(datetime,'" + dt.ToString() + "',101) " +
                                "   and (Matter.MatDateLastPaymt)<convert(datetime,'" + dt.ToString() + "',101) ";
                }

                MatterSql = MatterSql + getTimeKeepers() + getClients() + getPCs() + getdtException() + getbtException() + getCliException() + ") ";

                _jurisUtility.ExecuteNonQueryCommand(0, MatterSql);

                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                toolStripStatusLabel.Text = "Executing Matter Closer....";
                statusStrip.Refresh();
                UpdateStatus("Executing Matter Closer", 4, 5);

                DialogResult r3 = MessageBox.Show(sumToClose.ToString() + " matters closed.",
                                   "Matters Closed", MessageBoxButtons.OK,
                                   MessageBoxIcon.Information,
                                   MessageBoxDefaultButton.Button1);



                Cursor.Current = Cursors.Default;
                Application.DoEvents();
                toolStripStatusLabel.Text = "Matter Closer Completed...." + sumToClose.ToString() + " matters closed. " + sumExcept.ToString() + " matters could not be closed due to exceptions.";
                statusStrip.Refresh();
                UpdateStatus("Matter Closer Completed", 5, 5);
            }




        }

        
        



        private bool VerifyFirmName()
        {
            //    Dim SQL     As String
            //    Dim rsDB    As ADODB.Recordset
            //
            //    SQL = "SELECT CASE WHEN SpTxtValue LIKE '%firm name%' THEN 'Y' ELSE 'N' END AS Firm FROM SysParam WHERE SpName = 'FirmName'"
            //    Cmd.CommandText = SQL
            //    Set rsDB = Cmd.Execute
            //
            //    If rsDB!Firm = "Y" Then
            return true;
            //    Else
            //        VerifyFirmName = False
            //    End If

        }



        private static bool IsDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum; 
        }

        private void WriteLog(string comment)
        {
            string sql = "Insert Into UtilityLog(ULTimeStamp,ULWkStaUser,ULComment) Values(convert(datetime,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'),cast('" +  GetComputerAndUser() + "' as varchar(100))," + "cast('" + comment.ToString() + "' as varchar(8000)))";
            _jurisUtility.ExecuteNonQueryCommand(0, sql);
        }

        private string GetComputerAndUser()
        {
            var computerName = Environment.MachineName;
            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var userName = (windowsIdentity != null) ? windowsIdentity.Name : "Unknown";
            return computerName + "/" + userName;
        }

        /// <summary>
        /// Update status bar (text to display and step number of total completed)
        /// </summary>
        /// <param name="status">status text to display</param>
        /// <param name="step">steps completed</param>
        /// <param name="steps">total steps to be done</param>
        private void UpdateStatus(string status, long step, long steps)
        {
            labelCurrentStatus.Text = status;

            if (steps == 0)
            {
                progressBar.Value = 0;
                labelPercentComplete.Text = string.Empty;
            }
            else
            {
                double pctLong = Math.Round(((double)step/steps)*100.0);
                int percentage = (int)Math.Round(pctLong, 0);
                if ((percentage < 0) || (percentage > 100))
                {
                    progressBar.Value = 0;
                    labelPercentComplete.Text = string.Empty;
                }
                else
                {
                    progressBar.Value = percentage;
                    labelPercentComplete.Text = string.Format("{0} percent complete", percentage);
                }
            }
        }
        private void DeleteLog()
        {
            string AppDir = Path.GetDirectoryName(Application.ExecutablePath);
            string filePathName = Path.Combine(AppDir, "VoucherImportLog.txt");
            if (File.Exists(filePathName + ".ark5"))
            {
                File.Delete(filePathName + ".ark5");
            }
            if (File.Exists(filePathName + ".ark4"))
            {
                File.Copy(filePathName + ".ark4", filePathName + ".ark5");
                File.Delete(filePathName + ".ark4");
            }
            if (File.Exists(filePathName + ".ark3"))
            {
                File.Copy(filePathName + ".ark3", filePathName + ".ark4");
                File.Delete(filePathName + ".ark3");
            }
            if (File.Exists(filePathName + ".ark2"))
            {
                File.Copy(filePathName + ".ark2", filePathName + ".ark3");
                File.Delete(filePathName + ".ark2");
            }
            if (File.Exists(filePathName + ".ark1"))
            {
                File.Copy(filePathName + ".ark1", filePathName + ".ark2");
                File.Delete(filePathName + ".ark1");
            }
            if (File.Exists(filePathName ))
            {
                File.Copy(filePathName, filePathName + ".ark1");
                File.Delete(filePathName);
            }

        }

            

        private void LogFile(string LogLine)
        {
            string AppDir = Path.GetDirectoryName(Application.ExecutablePath);
            string filePathName = Path.Combine(AppDir, "VoucherImportLog.txt");
            using (StreamWriter sw = File.AppendText(filePathName))
            {
                sw.WriteLine(LogLine);
            }	
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            DoDaFix();
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void rbDays_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDays.Checked == true)
            { cbDays.Visible = true;
                dtSince.Visible = false;
                rbStart.Checked = false;
            }
            else
             {
                cbDays.Visible = false;
                dtSince.Visible = true;
                rbStart.Checked = true;
            }

        }

        private void rbStart_CheckedChanged(object sender, EventArgs e)
        {

            if (rbStart.Checked == true)
            {
                cbDays.Visible = false;
                dtSince.Visible = true;
                rbDays.Checked = false;
            }
            else
            {
                cbDays.Visible = true;
                dtSince.Visible = false;
                rbDays.Checked = true;
            }
        }

        private void btExceptionRpt_Click(object sender, EventArgs e)
        {


            string SQLExc = @"Select dbo.jfn_formatclientcode(clicode) as Client, Clireportingname, dbo.jfn_formatmattercode(matcode) as Matter,MatPracticeClass as PC,
            Matreportingname, empinitials as BillTkpr, OrigTkpr, matsysnbr,convert(varchar(10),matdateopened,101) as OpenDate,
            case when matstatusflag='O' then 'Open' when matstatusflag='F' then 'Final Bill Sent' else matstatusflag end as Status, 
            case when convert(varchar(10), matdatelastwork,101)='01/01/1900' then 'Never' else convert(varchar(10), matdatelastwork,101) end  as LastWorked,
            case when  convert(varchar(10), matdatelastbill,101)='01/01/1900' then 'Never' else convert(varchar(10), matdatelastbill,101) end as LastBIlled,
            case when convert(varchar(10),  matdatelastexp,101)='01/01/1900' then 'Never' else convert(varchar(10),  matdatelastexp,101) end as LastExpense,
            case when convert(varchar(10), matdatelastpaymt,101)='01/01/1900' then 'Never' else convert(varchar(10), matdatelastpaymt,101) end as LastPayment, unbilledtime, unbilledexp, unpostedtime, unpostedexp, unpostedvouchers, openvouchers, openbills, wip, ar, ppd, trust
            from matter
            inner join client on matclinbr=clisysnbr
            inner join billto on matbillto=billtosysnbr
            inner join employee on empsysnbr=billtobillingatty
            inner join (select morigmat, max(case when ot=1 then empinitials else '' end) + max(case when ot=2 then  ' ' +  empinitials else '' end) + 
            max(case when ot=3 then   ' ' +  empinitials else '' end) +  max(case when ot=4 then   ' ' +  empinitials else '' end) + max(case when ot=5 then   ' ' +  empinitials else '' end) as OrigTkpr
            from (select morigmat, empinitials, rank() over (Partition by morigmat order by empinitials) as OT
            from matorigatty inner join employee on morigatty=empsysnbr)MO
            group by morigmat)MO on matsysnbr=morigmat
            inner join (select matsysnbr as matsys, sum(unbilledtime) as UnbilledTime, sum(UnbilledExp) as UnbilledExp, sum(unpostedtime) as UnpostedTime, sum(unpostedexp) as UnpostedExp,
            sum(unpostedvouchers) as UnpostedVouchers, sum(openVouchers) as OpenVouchers, sum(openBills) as OpenBills, sum(wipbalance) as WIp, sum(arbalance) as AR, sum(ppdbalance) as PPD, sum(trustbalance) as trust
            from (select matsysnbr, 0 as UnbilledTime, 0 as UnbilledExp, 0 as UnpostedTime, 0 as UnpostedExp, 0 as UnpostedVouchers, 0 as OpenVouchers,0 as OpenBills, 0 as WIPBalance,0 as ARBalance, matppdbalance as PPDBalance, 0 as TrustBalance
            from matter
            union all select armmatter, 0, 0, 0, 0, 0, 0, count(armbillnbr) as OpenBills,  0 as WIp, sum(armbaldue) as ARBalance, 0 as PPD, 0 as Trust from armatalloc where armbaldue<>0 group by armmatter
            union all select utmatter, count(utid), 0, 0, 0, 0, 0, 0, sum(utamount), 0, 0, 0 from unbilledtime group by utmatter union all
            select uematter, 0, count(ueid), 0,0,0,0,0,sum(ueamount), 0, 0, 0 from unbilledexpense group by uematter
            union all  select tbdmatter, 0,0, count(tbdid),0,0,0,0,0,0,0,0 from timebatchdetail  where tbdposted='N' and tbdid not in (select tbdid from timeentrylink) group by tbdmatter
            union all  select mattersysnbr, 0,0, count(entryid),0,0,0,0,0,0,0,0 from timeentry where entrystatus<=6 group by mattersysnbr
            union all  select ebdmatter, 0,0, 0,count(ebdid),0,0,0,0,0,0,0 from expbatchdetail where ebdposted='N' and ebdid not in (select ebdid from ExpenseEntryLink) group by ebdmatter
            union all  select mattersysnbr, 0,0, 0,count(entryid),0,0,0,0,0,0,0 from expenseentry where entrystatus<=6  group by mattersysnbr
            union all  select vbmmatter, 0,0, 0,0,count(vbdid) as VchCount,0,0,0,0,0,0  from voucherbatchmatdist inner join voucherbatchdetail on vbdbatch=vbmbatch and vbdrecnbr=vbmrecnbr where vbdposted='N' group by vbmmatter
            union all  select vmmatter,0,0, 0,0,0, count(vmvoucher) as Vch,0,0,0,0,0 from voucher inner join vouchermatdist on vmvoucher=vchvouchernbr where vchstatus='O' and vmamount-vmamountpaid<>0 group by vmmatter 
            union all  select tamatter,0,0,0,0,0,0,0,0,0,0, sum(tabalance) from trustaccount group by tamatter) Mat group by matsysnbr)MatList on matsysnbr=matsys ";

            SQLExc = SQLExc + @" where (matstatusflag='O' or matstatusflag='F') and (wip <> 0.00 or UnbilledTime <> 0 or UnbilledExp <> 0 or UnpostedTime <> 0 or UnpostedExp <> 0 or UnpostedVouchers <> 0 or OpenVouchers <> 0 or openbills <> 0 or AR <> 0.00 or PPD <> 0.00 or trust <> 0.00) " ;
            if(rbDays.Checked==true)
                        {string DaySince = cbDays.Text;
                        SQLExc = SQLExc + @" and DATEDIFF(d, Matter.MatDateLastWork, GETDATE())>" + DaySince.ToString() +
                                " and DATEDIFF(d, Matter.MatDateLastExp, GETDATE())>" + DaySince.ToString() +
                                    "  and DATEDIFF(d, Matter.MatDateLastBill, GETDATE())>" + DaySince.ToString() +
                                    "   and DATEDIFF(d, Matter.MatDateLastPaymt, GETDATE())>" + DaySince.ToString();            }

            else
                        {string dt = dtSince.Text;
                        SQLExc = SQLExc + @" and (Matter.MatDateLastWork)<convert(datetime,'" + dt.ToString() + "',101) " +
                                " and (Matter.MatDateLastExp)<convert(datetime,'" + dt.ToString() + "',101) " +
                                    "  and (Matter.MatDateLastBill)<convert(datetime,'" + dt.ToString() + "',101) " +
                                    "   and (Matter.MatDateLastPaymt)<convert(datetime,'" + dt.ToString() + "',101) ";  }

            SQLExc = SQLExc + getTimeKeepers() + getClients() + getPCs() + getdtException() + getbtException() + getCliException();
            SQLExc = SQLExc + @" order by clicode, matcode";
            //TextWriter write = new StreamWriter(@"c:\intel\sql1.txt");
            //  write.WriteLine(SQLExc);
            //  write.Flush();
            //  write.Close();

         

            DataSet exceptions = _jurisUtility.RecordsetFromSQL(SQLExc);
            ReportDisplay rpds = new ReportDisplay(exceptions);
            rpds.Show();
        }

        private string getTimeKeepers()
        {
            string tkps = "";
            if (cbBT.SelectedIndex > 0)
            {
                tkps = " and empinitials in ('" + singleTimekeeper + "') ";
            }
            else
                tkps = " ";
            return tkps;
            
        }

        private string getClients()
        {
            string tkps = "";
            if (cbClient.SelectedIndex > 0)
            {
                tkps = " and dbo.jfn_formatclientcode(clicode) in ('" + singleClient + "') ";
            }
            else
                tkps = " ";
            return tkps;

        }

        private string getPCs()
        {
            string tkps = "";
            if (cbPC.SelectedIndex > 0)
            {
                tkps = " and matpracticeclass in ('" + singlePracticeClass + "') ";
            }
            else
                tkps = " ";
            return tkps;

        }


        private string getdtException()
        {
            string getdt = "";
            string dtExcept = dtOpen.Text;
            if (rbClose.Checked == true)
            {
                getdt = " and matdateopened<=convert(datetime,'"  + dtExcept.ToString() + "',101)" ;
            }
            else
                getdt = " ";
            return getdt;

        }


        private string getbtException()
        {
            string tkps = "";
            if (cbExcBT.SelectedIndex > 0)
            {
                tkps = " and empinitials not in ('" + ExceptTimekeeper + "') ";
            }
            else
                tkps = " ";
            return tkps;
        }

        private string getCliException()
        {
            string tkps = "";
            if (cbExcCli.SelectedIndex > 0)
            {
                tkps = " and dbo.jfn_formatclientcode(clicode) not in ('" + ExceptClient + "') ";
            }
            else
                tkps = " ";
            return tkps;
        }




        private void cbBT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBT.SelectedIndex > 0)
                singleTimekeeper = this.cbBT.GetItemText(this.cbBT.SelectedItem).Split(' ') [0];
        }

        private void cbClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbClient.SelectedIndex > 0)
                singleClient = this.cbClient.GetItemText(this.cbClient.SelectedItem).Split(' ')[0];
        }

        private void cbPC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPC.SelectedIndex > 0)
                singlePracticeClass = this.cbPC.GetItemText(this.cbPC.SelectedItem).Split(' ')[0];
        }
        private void cbExcBT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbExcBT.SelectedIndex > 0)
                ExceptTimekeeper = this.cbExcBT.GetItemText(this.cbExcBT.SelectedItem).Split(' ')[0];
        }

        private void cbExcCli_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbExcCli.SelectedIndex > 0)
                ExceptClient = this.cbExcCli.GetItemText(this.cbExcCli.SelectedItem).Split(' ')[0];
        }

        private void rbClose_CheckedChanged(object sender, EventArgs e)
        {
            if (rbClose.Checked == true)
            {             
                dtOpen.Visible = true;
               
            }
        }
    }
}
