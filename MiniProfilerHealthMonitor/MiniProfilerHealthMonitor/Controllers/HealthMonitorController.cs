using MiniProfilerHealthMonitor.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MiniProfilerHealthMonitor.Controllers
{
    public class HealthMonitorController : Controller
    {
        //
        // GET: /HealthMonitor/BoxAndWhisker
        public ActionResult BoxAndWhisker(BoxAndWhiskerSearchCriteria boxAndWhiskerSearchCriteria, int pageNumber = 1, int pageSize = 10, bool hasSearchBeenPerformed = false)
        {
            using (var context = new MiniProfilerEntities())
            {
                if(!hasSearchBeenPerformed)
                {
                    boxAndWhiskerSearchCriteria = new BoxAndWhiskerSearchCriteria
                    {
                        BeginDate = new DateTime(DateTime.Now.Year, 1, 1),
                        EndDate = new DateTime(DateTime.Now.Year, 12, 31),
                    };
                }

                ViewBag.BoxAndWhiskerSearchCriteria = boxAndWhiskerSearchCriteria;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;

                var results = context.Database.SqlQuery<BoxAndWhiskerDto>(@"
	                SELECT Quartile1.Name, FirstQuartileMinimum, SecondQuartileMinimum, (SecondQuartileMaximum + thirdQuartileMinimum) / 2 AS Median, ThirdQuartileMaximum, FourthQuartileMaximum, AverageDuration
	                FROM
	                (
		                SELECT MAX(Quartile1Inner.Name) as Name, MIN(Quartile1Inner.DurationMilliseconds) as FirstQuartileMinimum
		                FROM 
		                (
			                SELECT Name, DurationMilliseconds, NTILE(4) OVER(PARTITION BY Name ORDER BY DurationMilliseconds) as Quartile
			                FROM MiniProfilers with (nolock)
			                WHERE 
				                Name != '/'
				                AND [Started] > @beginDate
				                AND [Started] < @endDate 
		                ) AS Quartile1Inner
		                WHERE Quartile = 1
		                GROUP BY Name
	                ) AS Quartile1
	                JOIN
	                (
		                SELECT MAX(Quartile2Inner.Name) as Name, MIN(Quartile2Inner.DurationMilliseconds) as SecondQuartileMinimum, MAX(Quartile2Inner.DurationMilliseconds) as SecondQuartileMaximum
		                FROM 
		                (
			                SELECT Name, DurationMilliseconds, NTILE(4) OVER(PARTITION BY Name ORDER BY DurationMilliseconds) as Quartile
			                FROM MiniProfilers with (nolock)
			                WHERE 
				                Name != '/'
				                AND [Started] > @beginDate
				                AND [Started] < @endDate 
		                ) AS Quartile2Inner
		                WHERE Quartile = 2
		                GROUP BY Name
	                ) AS Quartile2
	                ON Quartile1.Name = Quartile2.Name
	                JOIN
	                (
		                SELECT MAX(Quartile3Inner.Name) as Name, MAX(Quartile3Inner.DurationMilliseconds) as ThirdQuartileMaximum, MIN(Quartile3Inner.DurationMilliseconds) as ThirdQuartileMinimum
		                FROM 
		                (
			                SELECT Name, DurationMilliseconds, NTILE(4) OVER(PARTITION BY Name ORDER BY DurationMilliseconds) as Quartile
			                FROM MiniProfilers with (nolock)
			                WHERE 
				                Name != '/'
				                AND [Started] > @beginDate
				                AND [Started] < @endDate
		                ) AS Quartile3Inner
		                WHERE Quartile = 3
		                GROUP BY Name
	                ) AS Quartile3
	                ON Quartile2.Name = Quartile3.Name
	                JOIN
	                (
		                SELECT MAX(Quartile4Inner.Name) as Name, MAX(Quartile4Inner.DurationMilliseconds) as FourthQuartileMaximum
		                FROM 
		                (
			                SELECT Name, DurationMilliseconds, NTILE(4) OVER(PARTITION BY Name ORDER BY DurationMilliseconds) as Quartile
			                FROM MiniProfilers with (nolock)
			                WHERE 
				                Name != '/'
				                AND [Started] > @beginDate
				                AND [Started] < @endDate 
		                ) AS Quartile4Inner
		                WHERE Quartile = 4
		                GROUP BY Name
	                ) AS Quartile4
	                ON Quartile3.Name = Quartile4.Name
                    JOIN
                    (
                        SELECT Name, AVG(DurationMilliseconds) as AverageDuration
                        FROM MiniProfilers with (nolock)
                        WHERE
                        	Name != '/'
				            AND [Started] > @beginDate
				            AND [Started] < @endDate 
                        GROUP BY Name
                    ) AS AverageDuration
                    ON Quartile4.Name = AverageDuration.Name
                    ORDER BY FourthQuartileMaximum desc
                    OFFSET (@pageNumber-1)*@pageSize ROWS
                    FETCH NEXT @pageSize ROWS ONLY",
                    new SqlParameter("beginDate", boxAndWhiskerSearchCriteria.BeginDate.ToString()), 
                    new SqlParameter("endDate",boxAndWhiskerSearchCriteria.EndDate.ToString()),
                    new SqlParameter("pageSize", pageSize),
                    new SqlParameter("pageNumber", pageNumber)
                ).ToList();


                //Get Total Item Count
                ViewBag.TotalItems = context.Database.SqlQuery<int>(@"
	                SELECT COUNT(1)
	                FROM
	                (
		                SELECT MAX(Quartile1Inner.Name) as Name, MIN(Quartile1Inner.DurationMilliseconds) as FirstQuartileMinimum
		                FROM 
		                (
			                SELECT Name, DurationMilliseconds, NTILE(4) OVER(PARTITION BY Name ORDER BY DurationMilliseconds) as Quartile
			                FROM MiniProfilers with (nolock)
			                WHERE 
				                Name != '/'
				                AND [Started] > @beginDate
				                AND [Started] < @endDate 
		                ) AS Quartile1Inner
		                WHERE Quartile = 1
		                GROUP BY Name
	                ) AS Quartile1
	                JOIN
	                (
		                SELECT MAX(Quartile2Inner.Name) as Name, MIN(Quartile2Inner.DurationMilliseconds) as SecondQuartileMinimum, MAX(Quartile2Inner.DurationMilliseconds) as SecondQuartileMaximum
		                FROM 
		                (
			                SELECT Name, DurationMilliseconds, NTILE(4) OVER(PARTITION BY Name ORDER BY DurationMilliseconds) as Quartile
			                FROM MiniProfilers with (nolock)
			                WHERE 
				                Name != '/'
				                AND [Started] > @beginDate
				                AND [Started] < @endDate 
		                ) AS Quartile2Inner
		                WHERE Quartile = 2
		                GROUP BY Name
	                ) AS Quartile2
	                ON Quartile1.Name = Quartile2.Name
	                JOIN
	                (
		                SELECT MAX(Quartile3Inner.Name) as Name, MAX(Quartile3Inner.DurationMilliseconds) as ThirdQuartileMaximum, MIN(Quartile3Inner.DurationMilliseconds) as ThirdQuartileMinimum
		                FROM 
		                (
			                SELECT Name, DurationMilliseconds, NTILE(4) OVER(PARTITION BY Name ORDER BY DurationMilliseconds) as Quartile
			                FROM MiniProfilers with (nolock)
			                WHERE 
				                Name != '/'
				                AND [Started] > @beginDate
				                AND [Started] < @endDate
		                ) AS Quartile3Inner
		                WHERE Quartile = 3
		                GROUP BY Name
	                ) AS Quartile3
	                ON Quartile2.Name = Quartile3.Name
	                JOIN
	                (
		                SELECT MAX(Quartile4Inner.Name) as Name, MAX(Quartile4Inner.DurationMilliseconds) as FourthQuartileMaximum
		                FROM 
		                (
			                SELECT Name, DurationMilliseconds, NTILE(4) OVER(PARTITION BY Name ORDER BY DurationMilliseconds) as Quartile
			                FROM MiniProfilers with (nolock)
			                WHERE 
				                Name != '/'
				                AND [Started] > @beginDate
				                AND [Started] < @endDate 
		                ) AS Quartile4Inner
		                WHERE Quartile = 4
		                GROUP BY Name
	                ) AS Quartile4
	                ON Quartile3.Name = Quartile4.Name
                    JOIN
                    (
                        SELECT Name, AVG(DurationMilliseconds) as AverageDuration
                        FROM MiniProfilers with (nolock)
                        WHERE
                        	Name != '/'
				            AND [Started] > @beginDate
				            AND [Started] < @endDate 
                        GROUP BY Name
                    ) AS AverageDuration
                    ON Quartile4.Name = AverageDuration.Name",
                    new SqlParameter("beginDate", boxAndWhiskerSearchCriteria.BeginDate.ToString()),
                    new SqlParameter("endDate", boxAndWhiskerSearchCriteria.EndDate.ToString())
                ).First();

                return View(results);
            }
        }

        public ActionResult ListActivity(int pageNumber = 1, int pageSize = 10)
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            using (var context = new MiniProfilerEntities())
            {
                using (var t = new TransactionScope(TransactionScopeOption.Required,
                                    new TransactionOptions
                                    {
                                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                                    }))
                {
                    var queryResults = context.MiniProfilers
                                                .Where(w => w.Name != "/")
                                                .OrderByDescending(o => o.Started)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .Select(s => new
                                                {
                                                    Id = s.Id,
                                                    Name = s.Name,
                                                    BeginTime = s.Started,
                                                    Duration = s.DurationMilliseconds
                                                })
                                                .ToList();

                    var returnResults = queryResults.Select(s => new ActivityDto()
                                                                    {
                                                                        Id = s.Id.ToString(),
                                                                        BeginTime = s.BeginTime,
                                                                        Duration = s.Duration,
                                                                        Name = s.Name
                                                                    }).ToList();

                    ViewBag.TotalItems = context.MiniProfilers
                                                .Where(w => w.Name != "/")
                                                .OrderByDescending(o => o.Started)
                                                .Select(s => new 
                                                {
                                                    Id = s.Id,
                                                    Name = s.Name,
                                                    BeginTime = s.Started,
                                                    Duration = s.DurationMilliseconds
                                                }).Count();



                    return View(returnResults);
                }
            }
        }

        public ActionResult ActivityQueries(string id)
        {
            using (var context = new MiniProfilerEntities())
            {
                var queries = context.MiniProfilerSqlTimings.Where(w => w.MiniProfilerId == new Guid(id)).OrderByDescending(o => o.DurationMilliseconds).ToList();

                var queryListToReturn = new List<ActivityQueryDto>();
                foreach (var query in queries)
                {
                    var queryParameters = context.MiniProfilerSqlTimingParameters.Where(w => w.ParentSqlTimingId == query.Id).ToList();

                    foreach(var param in queryParameters)
                    {
                        if (!param.Name.Contains("linq"))
                        {
                            if (param.DbType == "String" || param.DbType == "DateTime2" || param.DbType == "DateTime")
                            {
                                query.CommandString = query.CommandString.Replace(param.Name, "\'" + param.Value + "\'");
                            }
                            else
                            {
                                query.CommandString = query.CommandString.Replace(param.Name, param.Value);
                            }
                        }
                        else
                        {
                            if (param.DbType == "String" || param.DbType == "DateTime2" || param.DbType == "DateTime")
                            {
                                query.CommandString = query.CommandString.Replace("@" + param.Name, "\'" + param.Value + "\'");
                            }
                            else
                            {
                                query.CommandString = query.CommandString.Replace("@" + param.Name, param.Value);
                            }
                        }
                    }

                    queryListToReturn.Add(new ActivityQueryDto()
                                                            {
                                                                Command = query.CommandString,
                                                                Duration = query.DurationMilliseconds,
                                                                Id = query.Id.ToString()
                                                            }
                                            );
                }


                return View(queryListToReturn);
            }
        }

        public ActionResult RouteUseFrequency(RouteUseFrequencySearchCriteria routeUseFrequencySearchCriteria, int pageNumber = 1, int pageSize = 10, bool hasSearchBeenPerformed = false)
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            if (!hasSearchBeenPerformed)
            {
                routeUseFrequencySearchCriteria = new RouteUseFrequencySearchCriteria
                {
                    BeginDate = new DateTime(DateTime.Now.Year, 1, 1),
                    EndDate = new DateTime(DateTime.Now.Year, 12, 31),
                };
            }

            ViewBag.RouteUseFrequencySearchCriteria = routeUseFrequencySearchCriteria;


            using (var context = new MiniProfilerEntities())
            {
                using (var t = new TransactionScope(TransactionScopeOption.Required,
                                                    new TransactionOptions
                                                    {
                                                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                                                    }))
                {
                    var results = (from miniProfiler in context.MiniProfilers
                                   where miniProfiler.Name != "/"
                                   where miniProfiler.Started > routeUseFrequencySearchCriteria.BeginDate
                                   where miniProfiler.Started < routeUseFrequencySearchCriteria.EndDate
                                   group miniProfiler by miniProfiler.Name into groupedMiniProfiler
                                   orderby groupedMiniProfiler.Count() descending
                                   select new RouteFrequencyDto
                                   {
                                       Name = groupedMiniProfiler.Key,
                                       FrequencyCount = groupedMiniProfiler.Count()
                                   })
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();

                    ViewBag.TotalItems = (from miniProfiler in context.MiniProfilers
                                          where miniProfiler.Name != "/"
                                          where miniProfiler.Started > routeUseFrequencySearchCriteria.BeginDate
                                          where miniProfiler.Started < routeUseFrequencySearchCriteria.EndDate
                                          group miniProfiler by miniProfiler.Name into groupedMiniProfiler
                                          orderby groupedMiniProfiler.Count() descending
                                          select new RouteFrequencyDto
                                          {
                                              Name = groupedMiniProfiler.Key,
                                              FrequencyCount = groupedMiniProfiler.Count()
                                          })
                                          .Count();


                    return View(results);
                }
            }
        }




	}
}