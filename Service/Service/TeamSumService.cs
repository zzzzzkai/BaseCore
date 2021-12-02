using DataModel;
using Repository.IRepository;
using Service.IService;
using ServiceExt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Service
{
    public class TeamSumService:BaseService<TeamSum>,ITeamSumService
    {
        private readonly ITeamSumRepository _teamSumRepository;
        private readonly IOrdersRepository _orderRepository;
        private readonly ISumTimeRepository _sumTimeRepository;
        private readonly ITj_LncRepository _tj_LncRepository;
        public TeamSumService(ITeamSumRepository teamSumRepository,IOrdersRepository orderRepository, ISumTimeRepository sumTimeRepository , ITj_LncRepository tj_LncRepository) 
        {
            _teamSumRepository = teamSumRepository;
            _baseRepository = _teamSumRepository;
            _orderRepository = orderRepository;
            _sumTimeRepository = sumTimeRepository;
            _tj_LncRepository = tj_LncRepository;
        }

        public object Getlncmenu(string Keyvalue)
        {

            try
            {
                var model = _tj_LncRepository.FindAll().Where(x=>x.lnc_State=="T").ToList();
                if (!string.IsNullOrEmpty(Keyvalue))
                {
                    model = model.Where(x => x.lnc_Code.Contains(Keyvalue) || x.lnc_Name.Contains(Keyvalue)).ToList();


                }
                return model;
           

            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<TeamSum> GetMonthofDaylnccodeT(string date_Time, string lnccode)
        {
            try
            {
                var datetime = Convert.ToDateTime(date_Time);
                var first = datetime.AddDays(1 - datetime.Day);
                var last = datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
                var num = DateTime.DaysInMonth(datetime.Year, datetime.Month);
                var date = from a in _teamSumRepository.FindAll()
                           join b in _tj_LncRepository.FindAll()
                      
                           on a.team_LncCode equals b.lnc_Code   
                        
                           where a.team_Date >= first && a.team_Date <= last && a.team_LncCode == lnccode
                          
                           select new
                           {
                               a.team_Date,
                               a.team_Flag,
                               a.team_LncCode,
                               a.team_Week,
                               a.team_Surplus,
                               a.team_Already,
                               a.team_Sum,
                               b.lnc_Name
                           };
                List < TeamSum > list = new List<TeamSum>();
                for(int i = 0; i < num; i++)
                {
                    var dayNo=date.Where(x => x.team_Date == first.AddDays(i));

                    if (dayNo.Count()!=0)
                    {

                        TeamSum teamSum = new TeamSum();
                        var team_Sum = 0;
                        var team_Surplus = 0;
                        var team_Already = 0;
                        foreach(var item in dayNo)
                        {
                            teamSum.team_Flag = item.team_Flag;
                            teamSum.team_Date = item.team_Date;
                            team_Sum += Convert.ToInt32(item.team_Sum);
                            team_Surplus += Convert.ToInt32(item.team_Surplus);
                            team_Already += Convert.ToInt32(item.team_Already);

                        }
                        teamSum.team_Sum = team_Sum;
                        teamSum.team_Surplus = team_Surplus;
                        teamSum.team_Already = team_Already;
                       

                        //teamSum.team_Flag = dayNo.team_Flag;
                        //teamSum.team_LncCode = dayNo.team_LncCode;
                        //teamSum.team_Sum = dayNo.team_Sum;
                        //teamSum.team_Surplus = dayNo.team_Surplus;
                        //teamSum.team_Already = dayNo.team_Already;

                        string shift = first.Date.AddDays(i).DayOfWeek.ToString();
                        switch (shift)
                        {
                            case "Monday":
                                teamSum.team_Week = "周一";
                                break;
                            case "Tuesday":
                                teamSum.team_Week = "周二";
                                break;
                            case "Wednesday":
                                teamSum.team_Week = "周三";
                                break;
                            case "Thursday":
                                teamSum.team_Week = "周四";
                                break;
                            case "Friday":
                                teamSum.team_Week = "周五";
                                break;
                            case "Saturday":
                                teamSum.team_Week = "周六";
                                break;
                            case "Sunday":
                                teamSum.team_Week = "周日";
                                break;
                            default:
                                break;
                        }
                        list.Add(teamSum);
                    }
                    else
                    {
                        var item = new TeamSum();
                        item.team_Date = first.Date.AddDays(i);
                        item.team_Already = 0;
                        item.team_Flag = "F";
                        item.team_Sum = 0;
                        item.team_Surplus = 0;
                        string shift = first.Date.AddDays(i).DayOfWeek.ToString();
                   
                        switch (shift)
                        {
                            case "Monday":
                                item.team_Week = "周一";
                                break;
                            case "Tuesday":
                                item.team_Week = "周二";
                                break;
                            case "Wednesday":
                                item.team_Week = "周三";
                                break;
                            case "Thursday":
                                item.team_Week = "周四";
                                break;
                            case "Friday":
                                item.team_Week = "周五";
                                break;
                            case "Saturday":
                                item.team_Week = "周六";
                                break;
                            case "Sunday":
                                item.team_Week = "周日";
                                break;
                            default:
                                break;
                        }

                        //周日为休假
                        if (item.team_Week == "周日")
                        {
                            item.team_Flag = "T";
                        }

                        list.Add(item);
                    }

                }

                return list;

            }
            catch(Exception ex)
            {
                throw ex;
            }
             
        }

        //团检总号源显示
        public List<TeamSum> GetMonthofDayT(string date_Time)
        {
            try
            {
                var datetime = Convert.ToDateTime(date_Time);
                var first = datetime.AddDays(1 - datetime.Day);
                var last = datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
                var num = DateTime.DaysInMonth(datetime.Year, datetime.Month);
                var query = _teamSumRepository.FindListByClause(x => x.team_Date >= first && x.team_Date <= last).GroupBy(x => new { x.team_Date }).ToList()
                    .Select(x => new TeamSum
                    {
                        team_Date = x.Key.team_Date,
                        team_Sum = x.Sum(y => y.team_Sum),
                        team_Already = x.Sum(y => y.team_Already),
                        team_Surplus = x.Sum(y => y.team_Surplus),
                        team_Flag="F",

                    });
                List<TeamSum> list = new List<TeamSum>();
                for(int i = 0; i < num; i++)
                {
                    var dayNo = query.FirstOrDefault(x => x.team_Date == first.AddDays(i));
     


                    if (dayNo != null)
                    {
                        string shift = first.Date.AddDays(i).DayOfWeek.ToString();
                        TeamSum teamSum = new TeamSum();
                        teamSum.team_Date = dayNo.team_Date;
                        teamSum.team_Sum = dayNo.team_Sum;
                        teamSum.team_Already = dayNo.team_Already;
                        teamSum.team_Surplus = dayNo.team_Surplus;
                        teamSum.team_Flag = dayNo.team_Flag;
                        switch (shift)
                        {
                            case "Monday":
                                teamSum.team_Week = "周一";
                                break;
                            case "Tuesday":
                                teamSum.team_Week = "周二";
                                break;
                            case "Wednesday":
                                teamSum.team_Week = "周三";
                                break;
                            case "Thursday":
                                teamSum.team_Week = "周四";
                                break;
                            case "Friday":
                                teamSum.team_Week = "周五";
                                break;
                            case "Saturday":
                                teamSum.team_Week = "周六";
                                break;
                            case "Sunday":
                                teamSum.team_Week = "周日";
                                break;
                            default:
                                break;
                        }
                        list.Add(teamSum);

                    }
                    else
                    {
                        TeamSum item = new TeamSum();
                        item.team_Sum = 0;
                        item.team_Date = first.AddDays(i);
                        item.team_Surplus = 0;
                        item.team_Already = 0;
                        item.team_Flag = "F";
                        // string shift = first.Date.AddDays(i).DayOfWeek.ToString();
                        string shift = first.Date.AddDays(i).DayOfWeek.ToString();
                        switch (shift)
                        {
                            case "Monday":
                                item.team_Week = "周一";
                                break;
                            case "Tuesday":
                                item.team_Week = "周二";
                                break;
                            case "Wednesday":
                                item.team_Week = "周三";
                                break;
                            case "Thursday":
                                item.team_Week = "周四";
                                break;
                            case "Friday":
                                item.team_Week = "周五";
                                break;
                            case "Saturday":
                                item.team_Week = "周六";
                                break;
                            case "Sunday":
                                item.team_Week = "周日";
                                break;
                            default:
                                break;
                        }
                        list.Add(item);
                    }
                }
                return list;



            }catch(Exception ex)
            {
                throw ex;
            }
           
        }
        #region 团体号源（无时段项目）
        ///// <summary>
        ///// 团体号源查询
        ///// </summary>
        ///// <param name="lnccode">单位编码</param>
        ///// <param name="start">号源查询开始时间</param>
        ///// <param name="end">号源查询结束时间</param>
        ///// <returns></returns>
        //public object GetTeamSumList(string lnccode, int start, int end)
        //{
        //    var startDay = DateTime.Now.AddDays(start).ToString("yyyy-MM-dd");
        //    var endDay = DateTime.Now.AddDays(end).ToString("yyyy-MM-dd");
        //    DateTime st = Convert.ToDateTime(startDay);
        //    DateTime et = Convert.ToDateTime(endDay);
        //    var queryList = _teamSumRepository.FindListByClause(x => x.team_Date >= st && x.team_Date <= et && x.team_LncCode == lnccode, "team_Date").ToList()
        //        .Select(z => new
        //        {
        //            team_Date = z.team_Date.Value.ToString("yyyy-MM-dd"),
        //            z.team_Sum,
        //            z.team_Surplus,
        //            z.team_Already,
        //            z.team_Flag,
        //        }).ToList();
        //    return queryList;
        //}
        #endregion
        #region 团体号源（有时段项目）
        /// <summary>
        /// 团体号源查询（有时段项目 --不需要的项目注释掉）
        /// </summary>
        /// <param name="lnccode"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public object GetTeamSumList(string lnccode, int start, int end)
        {
            var startDay = DateTime.Now.AddDays(start).ToString("yyyy-MM-dd");
            var endDay = DateTime.Now.AddDays(end).ToString("yyyy-MM-dd");
            DateTime st = Convert.ToDateTime(startDay);
            DateTime et = Convert.ToDateTime(endDay);
            var queryList = _teamSumRepository.FindListByClause(x => x.team_Date >= st && x.team_Date <= et && x.team_LncCode == lnccode)
                .GroupBy(y => new { y.team_Date, y.team_Flag }).Select(z =>
                       new
                       {
                           team_Date = z.Key.team_Date.Value.ToString("yyyy-MM-dd"),//号源日期
                           team_Sum = z.Sum(a => a.team_Sum),//设定号源
                           team_Surplus = z.Sum(a => a.team_Surplus),//剩余号源
                           team_Already = z.Sum(a => a.team_Already),//已约号源
                           z.Key.team_Flag//休假日期
                       }).ToList();
            return queryList;
        }

        /// <summary>
        /// 团体号源修改
        /// </summary>
        /// <param name="date_Time"></param>
        /// <returns></returns>
        public bool TeamSumUpdate(DateTime date_Time, string lnc_code, string sumtime_Code)
        {
            //获取当前时间段已约订单数量
            var oldlst = _orderRepository.FindListByClause(x => x.begin_Time == date_Time && x.lnc_Code == lnc_code && x.type == "group" && (x.state == "F" || x.state == "export") && x.sumtime_Code==sumtime_Code).Count();
            //获取当前时间段号源数
            var sum = _teamSumRepository.FindByClause(x => x.team_Date == date_Time && x.team_LncCode == lnc_code && x.sumtime_Code == sumtime_Code);
            if (sum == null)
            {
                return false;
            }
            sum.team_Already = oldlst;
            if (sum.team_Already < 0)
            {
                sum.team_Already = 0;
            }
            sum.team_Surplus = sum.team_Sum - sum.team_Already;
            if (sum.team_Already < 0)
            {
                sum.team_Surplus = 0;
                return false;
            }
            _teamSumRepository.Update(sum);
            return true;
        }
        #endregion

        /// 获取设置预留号源数据
        public object GetTeamSumnosumm(string team_Date, string team_LncCode,string type)
        {
            try
            { string  team_Flagg = "F";

                DateTime dt = new DateTime();
                DateTime.TryParse(team_Date, out dt);
                string shift = Convert.ToDateTime(dt).DayOfWeek.ToString();
                if(shift== "Sunday")
                {
                    team_Flagg = "T";

                }
                var bookNoList = from t in _sumTimeRepository.FindListByClause(x=>x.sumtime_Flag==type)
                                 join p in _teamSumRepository.FindListByClause(x => x.team_Date == dt && x.team_LncCode == team_LncCode)
                                 on t.sumtime_Code equals p.sumtime_Code into temp
                                 from tp in temp.DefaultIfEmpty().ToList()
                                 select new
                                 {   t.sumtime_EndTime,
                                     t.sumtime_BegTime,
                                     t.sumtime_Name,
                                     t.sumtime_Code,
                                     team_Sum = tp?.team_Sum ?? 0,
                                     team_Already = tp?.team_Already ?? 0,
                                     team_Surplus = tp?.team_Surplus ?? 0,
                                     team_Flag = tp?.team_Flag ?? team_Flagg
                                 };
        
                return bookNoList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool UpdateBooknoTotalTj(List<TeamSum> pData, ref string error)
        {
            error = string.Empty;
            try
            {
                foreach(var item in pData)
                {
                    DateTime times = Convert.ToDateTime(item.StartTime);
                    int nums = Convert.ToDateTime(item.EndTime).Subtract(Convert.ToDateTime(item.StartTime)).Days;
                    for(int i = 0; i <= nums; i++)
                    {
                        var bookdate = Convert.ToDateTime(times.AddDays(i).ToString("yyyy-MM-dd"));
                        var result = _teamSumRepository.FindByClause(x => x.team_Date == bookdate && x.team_LncCode == item.team_LncCode && x.sumtime_Code == item.sumtime_Code);
                        if (result == null)
                        {
                            TeamSum sum = new TeamSum();
                            sum.team_Sum = item.team_Sum;
                            sum.team_Already = 0;
                            sum.team_Date = bookdate;
                            sum.team_Surplus = item.team_Sum;
                            sum.team_Flag = item.team_Flag;
                            sum.team_LncCode = item.team_LncCode;
                            sum.sumtime_Code = item.sumtime_Code;
                            string shift = Convert.ToDateTime(bookdate).DayOfWeek.ToString();
                            switch (shift)
                            {
                                case "Monday":
                                    sum.team_Week = "1";
                                    break;
                                case "Tuesday":
                                    sum.team_Week = "2";
                                    break;
                                case "Wednesday":
                                    sum.team_Week = "3";
                                    break;
                                case "Thursday":
                                    sum.team_Week = "4";
                                    break;
                                case "Friday":
                                    sum.team_Week = "5";
                                    break;
                                case "Saturday":
                                    sum.team_Week = "6";
                                    break;
                                case "Sunday":
                                    sum.team_Week = "7";
                                    break;
                                default:
                                    break;
                            }
                            if (sum.team_Week == "7"&& nums>0)
                            {
                                continue;
                             
                            }
                            else { 
                            _teamSumRepository.Insert(sum);
                            }
                        }
                        else
                        {
                            if (result.team_Flag == "T" && nums > 0)
                            {
                                continue;
                            }
                            if (item.team_Flag == "T")
                            {
                                result.team_Flag = item.team_Flag;
                            }
                            else
                            {


                                result.team_Flag = item.team_Flag;
                                result.team_Surplus += item.team_Sum - result.team_Sum;
                                result.team_Sum = item.team_Sum;
                            }
                            _teamSumRepository.Update(result);
                        }
                    }
                }
                return true;
            }catch(Exception ex)
            {
                LogHelper.Error("个人后台号源修改失败", ex.Message);
                error = ex.Message;
                return false;
            }
          
        }

  

        public  List<SumTime> Timeslot(SumTime e)
        {
            var  model = _sumTimeRepository.FindListByClause(x => x.sumtime_Flag == e.sumtime_Flag).ToList();
            return model;
        }

    }

}
