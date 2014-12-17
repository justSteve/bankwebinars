using Moq;
using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;

namespace CUWebinars.Web.Tests.Infrastructure
{
    internal class WebTestHelpers
    {
        internal const string ReferrerAddress = "google.com";
        private static WebTestsGlobalConfig _webTestsGlobals = WebTestsGlobalConfig.WebTestsGlobalConfigSingleton;

        internal static HttpContextBase GetMockedHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            //RouteConfig.RegisterRoutes(RouteTable.Routes);

            var requestContext = new Mock<RequestContext>();
            requestContext.Setup(x => x.HttpContext).Returns(context.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(user.Object);
            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            identity.Setup(id => id.IsAuthenticated).Returns(true);
            //identity.Setup(id => id.Name).Returns("test");
            requestContext.Setup(x => x.RouteData).Returns(new RouteData());

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            request.Setup(req => req.Url).Returns(new Uri(_webTestsGlobals.SiteUrl));
            request.Setup(req => req.RequestContext).Returns(requestContext.Object);
            request.Setup(req => req.UrlReferrer).Returns(new Uri(_webTestsGlobals.SiteUrl));
            requestContext.Setup(x => x.RouteData).Returns(new RouteData());
            request.SetupGet(req => req["referred"]).Returns(ReferrerAddress);

            //  we also need to assign a value to HttpContext.Current as it is used in the AppHelper.GetUserAuditInfo method
            //HttpContext.Current = new HttpContext(
            //    new HttpRequest(string.Empty, _webTestsGlobals.SiteUrl, string.Empty),
            //    new HttpResponse(new StringWriter())
            //    );

            return context.Object;
        }
    }
}
