using Entity;
using Infrastructure.Utils;
using Log;
using Microsoft.Extensions.Options;
using Pomelo.AspNetCore.TimedJob;

namespace apicore.Jobs
{
    public class DailyStatJob : Job
    {
        private readonly SiteOptions _options;
        public DailyStatJob(IOptions<SiteOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// 每隔1小时更新一次
        /// </summary>
        [Invoke(Begin = "2018-05-18 22:00", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Generate()
        {
            var genDailyStat = $"{_options.Host}:{_options.Port}/Api/Bill/GenDailyStat";
            var result = HttpHelper.Get(genDailyStat);
            LogFactory.GetLogger().AddProperty("Task", "GenDailyStat").Info(result);

            var genStat = $"{_options.Host}:{_options.Port}/Api/Bill/GenStat";
            result = HttpHelper.Get(genStat);
            LogFactory.GetLogger().AddProperty("Task", "GenStat").Info(result);

            var genIncomeStat = $"{_options.Host}:{_options.Port}/Api/Bill/GenIncomeStat";
            result = HttpHelper.Get(genIncomeStat);
            LogFactory.GetLogger().AddProperty("Task", "GenIncomeStat").Info(result);
        }
    }
}
