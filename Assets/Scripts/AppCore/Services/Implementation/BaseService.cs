using Cysharp.Threading.Tasks;
using VContainer;
using Debug = UnityEngine.Debug;

/// <summary>
/// Base class for Service implementations.
/// </summary>
public abstract class BaseService
{
    /// <summary>
    /// The <see cref="ISerializationService"/> instance.
    /// </summary>
    [Inject] protected ISerializationService serializationService;
    
    /// <summary>
    /// Generic method for executing requests, usually calls repository method.
    /// </summary>
    /// <param name="request">The <see cref="UniTask{T}"/> request to call.</param>
    /// <param name="requestName">The request name for debugging purposes.</param>
    /// <typeparam name="TResultType">The type of the result</typeparam>
    /// <returns>The <see cref="TResultType"/> instance.</returns>
    protected async UniTask<TResultType> DoRequest<TResultType>(UniTask<(bool, string)> request, string requestName)
        where TResultType : class
    {
        var (success, dataAsText) = await request;

        if (!success)
        {
            if (Launch.Instance.showDebugBackend) Debug.LogWarning($"{GetType()}::{request} failed dataAsText = {dataAsText}");

            return null;
        }

        if (Launch.Instance.showDebugBackend) Debug.Log($"{GetType()}::{requestName} dataAsText= {dataAsText}");

        var deserialized = serializationService.Deserialize<TResultType>(dataAsText);

        if (Launch.Instance.showDebugBackend) Debug.Log($"{GetType()}::{requestName} deserialized= {deserialized}");

        return deserialized;
    }

}

