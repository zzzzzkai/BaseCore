using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using PeHubCore.Config;
using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.FileProviders;
using log4net.Repository;
using log4net;
using log4net.Config;
using Autofac;
using System.Reflection;
using AspectCore.Extensions.Autofac;
using Swashbuckle.AspNetCore.Filters;
using ServiceExt;
using Microsoft.AspNetCore.Http;
using Senparc.Weixin;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin.RegisterServices;
using Microsoft.Extensions.Options;
 
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.CO2NET;
using Senparc.CO2NET.AspNet;
using Senparc.Weixin.TenPay;
using PeHubCore.Middleware;
using AspNetCoreRateLimit;

namespace PeHubCore
{
    public class Startup
    {
        public static ILoggerRepository repository { get; set; }

        public static IContainer AutofacContainer;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            repository = LogManager.CreateRepository("rollingAppender");
            XmlConfigurator.Configure(repository, new System.IO.FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();// ֧�� NewtonsoftJson 
            //services.AddMvcCore(options => {
            //    options.Filters.Add(typeof(ApiDataFilter)); // by type
            //    //options.Filters.Add(new ApiDataFilter()); // an instance
            //});
            //services.AddMvcCore()
            //.AddAuthorization();

            //����
            services.AddMemoryCache();
            //ʹ��session
            services.AddSession();

            #region ��������
            //��Ҫ��appsettings.json�м�������

            services.AddOptions();

            //�洢IP�����������ù���

            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            //�����ĵ��������3.x�汾��breaking change,Ҫ����

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            #endregion

            #region Ids ��Ȩ�������� ����������� ��ids��Ȩ֮ǰ

            services.AddIdentityServer(options =>
                {
                    options.PublicOrigin = Appsettings.GetSectionValue("AppSettings:IdSHttpsUrl");
                })
               .AddDeveloperSigningCredential()
               .AddInMemoryIdentityResources(IdsConfig.GetIdentityResources())
               .AddInMemoryApiResources(IdsConfig.GetApis())
               .AddInMemoryClients(IdsConfig.GetClients())
               .AddTestUsers(IdsConfig.GetUsers());

 

           //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #endregion

            //ע���������Ȩ��API�ϲ���һ����

            #region ����api���� Ids��Ȩ ע�������Ҫ�ŵ�ǰ��

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    //��������Ҫ��д����
                    //var url = Configuration.GetValue<string>("URLS");
                    var url = Appsettings.GetSectionValue("AppSettings:IdSHttpsUrl");
                    options.Authority = url;
                    options.RequireHttpsMetadata = false;
                    options.Audience = "api1"; //ָ��ֻʹ���ĸ������򣬲�����ǰ�˿���

                });

            #endregion

            #region ���ÿ���
            services.AddCors(options => options.AddPolicy("Cors",
                builder =>
                //����1
                //builder.WithOrigins(Appsettings.GetSectionValue("AppSettings:CorsIPs").Split(','))
                //.AllowAnyMethod().AllowAnyHeader().AllowCredentials()));
                //����2
                builder.SetIsOriginAllowed(t => true)
                 .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));
            //builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

            #endregion

            #region ����swagger�����ĵ�
            #region �����汾����
            //��ȡapi�ֲ�汾�����Բ��ã�
            //services.AddApiVersioning(options =>
            //{
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.DefaultApiVersion = ApiVersion.Default;
            //    options.ReportApiVersions = true;
            //});
            // swagger
            //���API����˵��
            //services.AddSwaggerGen(option =>
            //{
            //    //option.SwaggerDoc("v1", new OpenApiInfo
            //    //{
            //    //    Version = "v1",
            //    //    Title = "sparktodo API",
            //    //    Description = "API for sparktodo",
            //    //    Contact = new OpenApiContact() { Name = "MIC", Email = "MIC@outlook.com" }
            //    //});

            //    //option.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "API V2" });

            //    option.DocInclusionPredicate((docName, apiDesc) =>
            //    {
            //        var versions = apiDesc.CustomAttributes()
            //            .OfType<ApiVersionAttribute>()
            //            .SelectMany(attr => attr.Versions);

            //        return versions.Any(v => $"v{v.ToString()}" == docName);
            //    });

            //    // �����ĵ��ļ�
            //    option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Startup).Assembly.GetName().Name}.xml"), true);
            //});
            //    //[ApiVersion("2")] ͨ�������
            #endregion
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Startup).Assembly.GetName().Name}.xml"), true);

                // ������ȨС��
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // ��header�����token�����ݵ���̨
                c.OperationFilter<SecurityRequirementsOperationFilter>();


                // ������ oauth2
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });
            });

            #endregion

            services.AddQueuePolicy(options =>
            {
                //��󲢷�������
                options.MaxConcurrentRequests = 10000;
                //������г�������
                options.RequestQueueLimit = 5000;
            });

            services.AddHttpClient();//��HttpClientע�����

            //Senparc.CO2NET ȫ��ע�ᣨ���룩
            services.AddSenparcGlobalServices(Configuration);
            //Senparc.Weixin ע��
            services.AddSenparcWeixinServices(Configuration);

            //�˴���Ҫ���ڽ��е��ýӿ�ʱ�ں�������»���  _  (һ��Ҫ���ϣ�����������޸Ŀ��Բ鿴Դ��ʾ������)
            services.AddCertHttpClient(Appsettings.GetSectionValue("SenparcWeixinSetting:TenPayV3_MchId") + "_",
                Appsettings.GetSectionValue("SenparcWeixinSetting:TenPayV3_MchId"),
                Appsettings.GetSectionValue("SenparcWeixinSetting:TenPayV3_CertPath"));
            //�˴�������Ӹ��� Cert ֤��
            services.AddControllersWithViews();

            //return AutofacConfigure.Register(services);

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if ((Appsettings.GetSectionValue("Middleware:IPLimitMiddleware") ?? "").ToUpper() == "TRUE")
            {
                app.UseConcurrencyLimiter();
                app.UseMiddleware<IPLimitMiddleware>();
            }

            #region ��ȡ��̬�ļ�
            app.UseStaticFiles();

            var pathkey = Appsettings.GetSectionValue("AppSettings:hospArry").Split(',');
            foreach (var item in pathkey)
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                    RequestPath = "/"+ item
                });
            }
            #endregion
            app.UseCookiePolicy();

            //ʹ�û���
            app.UseResponseCaching();

            //ע��Session����
            app.UseSession();

            #region ʹ�ÿ���
            app.UseCors("Cors");
            #endregion

            #region ʹ��swagger
            //ʹ��swagger
            app.UseSwagger();
            //Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                //option.SwaggerEndpoint("/swagger/v2/swagger.json", "API V1");
                option.RoutePrefix = "swagger";
                option.DocumentTitle = "sparktodo API";
            });

            #endregion

            #region ʹ��Ids���� 

            app.UseIdentityServer();

            #endregion

            //ʹ��https
            app.UseHttpsRedirection();

            //Ĭ����վ����ת��̬��ַ
            if (!string.IsNullOrEmpty(Appsettings.GetSectionValue("AppSettings:DefaultHtml")))
            {
                DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
                defaultFilesOptions.DefaultFileNames.Clear();
                defaultFilesOptions.DefaultFileNames.Add(Appsettings.GetSectionValue("AppSettings:DefaultHtml"));
                app.UseDefaultFiles(defaultFilesOptions);
            }


            app.UseStaticFiles();
            app.UseRouting();
 
            //app.UseMvc();

            #region Senparc.Weixin SDK

            // ���� CO2NET ȫ��ע�ᣬ���룡
            // ���� UseSenparcGlobal() �ĸ����÷��� CO2NET Demo��https://github.com/Senparc/Senparc.CO2NET/blob/master/Sample/Senparc.CO2NET.Sample.netcore3/Startup.cs
            var registerService = app.UseSenparcGlobal(env, senparcSetting.Value, globalRegister =>
            {
                #region CO2NET ȫ������
                #region APM ϵͳ����״̬ͳ�Ƽ�¼����

                //����APM�������ʱ�䣨Ĭ������¿��Բ������ã�
                Senparc.CO2NET.APM.Config.EnableAPM = true;//Ĭ���Ѿ�Ϊ�����������Ҫ�رգ�������Ϊ false
                Senparc.CO2NET.APM.Config.DataExpire = TimeSpan.FromMinutes(60);

                #endregion
                #endregion
            }, true)
                //ʹ�� Senparc.Weixin SDK
                .UseSenparcWeixin(senparcWeixinSetting.Value, weixinRegister =>
                {
                    #region ΢���������

                    /* ΢�����ÿ�ʼ
                    * 
                    * ���鰴������˳�����ע�ᣬ�����뽫������ڵ�һλ��
                    */

                    #region ע�ṫ�ںŻ�С���򣨰��裩

                    //ע�ṫ�ںţ���ע������                                                    -- DPBMARK MP
                    weixinRegister
                            .RegisterMpAccount(senparcWeixinSetting.Value, "���ں�")// DPBMARK_END
                                                                                          //�������⣬��Ȼ�����ڳ�������ط�ע�ṫ�ںŻ�С����
                                                                                          //AccessTokenContainer.Register(appId, appSecret, name);//�����ռ䣺Senparc.Weixin.MP.Containers
                    #endregion

                    #region ע��΢��֧�������裩        -- DPBMARK TenPay

                            //ע������΢��֧���汾��V3������ע������
                            .RegisterTenpayV3(senparcWeixinSetting.Value, "���ں�")//��¼��ͬһ�� SenparcWeixinSettingItem ������

                    #endregion                          // DPBMARK_END
                     ;
                    /* ΢�����ý��� */

                    #endregion
                });
            #endregion Senparc.Weixin SDK

            #region ��Ȩ��֤ids
            app.UseAuthentication();//��֤
            app.UseAuthorization();//��Ȩ
            #endregion
 
            app.UseMiddleware<Middleware.AskMiddleWare>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //ע��socket
            app.Map("/socket", WebSocketHelper.Map);

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {

            builder.RegisterTypes(Assembly.Load("Repository").GetExportedTypes()).AsImplementedInterfaces();
            builder.RegisterTypes(Assembly.Load("Service").GetExportedTypes()).AsImplementedInterfaces();
            //builder.RegisterType<ISession>();

            builder.RegisterDynamicProxy();
            // �ֶ�����
            builder.RegisterBuildCallback(container => AutofacContainer = container);

        }
    }
}
