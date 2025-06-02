using UnityEngine;

public class RepositionTile : MonoBehaviour
{
    [SerializeField]
    private LayerFlag checkLayer;

    [SerializeField]
    private float totalChunkSize = 40;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CheckLayer(checkLayer))
            return;

        if (!BattleSceneManager.Instance)
            return;

        Vector3 playerPos = BattleSceneManager.Instance.LeaderCharacter.transform.position;
        Vector3 pos = transform.position;

        float diffX = Mathf.Abs(playerPos.x - pos.x);
        float diffY = Mathf.Abs(playerPos.y - pos.y);

        Vector3 playerDir = BattleSceneManager.Instance.LeaderCharacter.Model.InputWrapper.Movement;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        if (diffX > diffY)
        {
            transform.Translate(dirX * totalChunkSize * Vector3.right);
        }
        else
        {
            transform.Translate(dirY * totalChunkSize * Vector3.up);
        }

        if (BattleSceneManager.Instance)
            BattleSceneManager.Instance.RecalculatePath();
    }
}
