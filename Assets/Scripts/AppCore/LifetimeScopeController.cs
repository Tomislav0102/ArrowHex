using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LifetimeScopeController : LifetimeScope
{
    [FoldoutGroup(FoldoutGroupNames.SetInInspector)]
    [SerializeField] protected bool persist = true;

    [FoldoutGroup(FoldoutGroupNames.SetInInspector)]
    [InlineEditor]
    [SerializeField] private ApiConfig apiConfig;

    protected override void Awake()
    {
        base.Awake();

        //Application.targetFrameRate = 72;
        // HTTPManager.Logger.Level = httpManagerLogLevel;

        if (persist)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);

        builder.RegisterInstance(apiConfig).AsImplementedInterfaces();
        builder.Register<ApiAccessToken>(Lifetime.Singleton);

        // register repositories
        builder.Register<IAuthRepository, AuthRepository>(Lifetime.Singleton);
        builder.Register<ICampaignRepository, CampaignRepository>(Lifetime.Singleton);
        builder.Register<IPlayerStatsRepository, PlayerStatsRepository>(Lifetime.Singleton);
        builder.Register<IMultiplayerRepository, MultiplayerRepository>(Lifetime.Singleton);

        // register services
        builder.Register<ISerializationService, SerializationService>(Lifetime.Singleton);
        builder.Register<IAuthService, AuthService>(Lifetime.Singleton);
        builder.Register<ICampaignService, CampaignService>(Lifetime.Singleton);
        builder.Register<IPlayerStatsService, PlayerStatsService>(Lifetime.Singleton);
        builder.Register<IMultiplayerService, MultiplayerService>(Lifetime.Singleton);
    }
}
