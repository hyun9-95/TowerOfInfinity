using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestBattleScene : BaseScene
{
    public override SceneDefine SceneType => SceneDefine.TestBattleScene;

    [SerializeField]
    private Transform spawnPosition;

    private void Awake()
    {
        TestPlayAsync().Forget();
    }

    private async UniTask TestPlayAsync()
    {
        await DataLoading();
    }

    private async UniTask DataLoading()
    {
        IntroController dataLoadingController = new();

        EditorDataLoader localDataLoader = new();
        localDataLoader.SetLocalJsonDataPath(PathDefine.Json);
        await localDataLoader.LoadData();

        DataManager.Instance.GenerateDataContainerByDataDic(localDataLoader.DicJsonByFileName);
    }
}
