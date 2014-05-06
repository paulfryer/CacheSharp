using System;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;

namespace CacheSharp.ASPNet
{
    public class CacheSharpSessionProvider : SessionStateStoreProviderBase
    { 
        private readonly IAsyncCache<string> cache;

        public CacheSharpSessionProvider(IAsyncCache<string> cache)
        {
            this.cache = cache;
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            throw new NotImplementedException();
        }

        public override void InitializeRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked,
            out TimeSpan lockAge, out object lockId,
            out SessionStateActions actions)
        {
            // set default out parameters
            locked = false;
            lockAge = new TimeSpan();
            lockId = null;
            actions = SessionStateActions.None;
            // try to get the session from cache.
            string sessionString = cache.GetAsync(id).Result;
            if (string.IsNullOrEmpty(sessionString))
                return null;
            var sessionItems = JsonConvert.DeserializeObject<SessionStateItemCollection>(sessionString);
            var data = new SessionStateStoreData(sessionItems, null, 60); // todo: set timeout.
            return data;
        }

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked,
            out TimeSpan lockAge,
            out object lockId, out SessionStateActions actions)
        {
            throw new NotImplementedException();
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            throw new NotImplementedException();
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item,
            object lockId, bool newItem)
        {
            throw new NotImplementedException();
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            cache.RemoveAsync(id);
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            throw new NotImplementedException();
        }

        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            var sessionItems = new SessionStateItemCollection();
            string sessionString = JsonConvert.SerializeObject(sessionItems);
            cache.PutAsync(context.Session.SessionID, sessionString, TimeSpan.FromMinutes(timeout));
            var data = new SessionStateStoreData(sessionItems, null, timeout);
            return data;
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            throw new NotImplementedException();
        }

        public override void EndRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}