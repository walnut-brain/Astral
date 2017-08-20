using Polly;

namespace Astral
{
    public static class Astral
    {
        private static Policy _defaultEventPublishPolicy = 
            Policy.NoOpAsync();
        
        private static readonly object _defaultEventPublishPolicyLocker = new object();

        public static Policy DefaultEventPublishPolicy
        {
            get
            {
                lock (_defaultEventPublishPolicyLocker)
                {
                    return _defaultEventPublishPolicy;    
                }
                
            }
            set
            {
                if (value != null)
                    value = Policy.NoOpAsync();
                lock (_defaultEventPublishPolicyLocker)
                {
                    _defaultEventPublishPolicy = value;
                }
            }
        }
    }
}