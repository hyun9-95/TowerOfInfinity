using UnityEngine;

public class RepositionTile : MonoBehaviour
{
    [SerializeField]
    private LayerFlag checkLayer;

    [SerializeField]
    private float totalChunkSize = 40;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CheckLayer(checkLayer) || !BattleSceneManager.Instance)
            return;

        Vector3 playerPos = BattleSceneManager.Instance.CurrentCharacter.transform.position;
        Vector3 tilePos = transform.position;

        float diffX = playerPos.x - tilePos.x;
        float diffY = playerPos.y - tilePos.y;

        // 절대값으로 주 이동 방향 판단
        float absDiffX = Mathf.Abs(diffX);
        float absDiffY = Mathf.Abs(diffY);

        Vector3 offset = Vector3.zero;

        if (absDiffX > absDiffY) // 수평 이동이 더 큼
        {
            // diffX가 양수면 플레이어가 타일의 오른쪽에 있다는 의미 -> 타일을 오른쪽으로 옮겨야 함
            // diffX가 음수면 플레이어가 타일의 왼쪽에 있다는 의미 -> 타일을 왼쪽으로 옮겨야 함
            offset.x = (diffX > 0 ? 1 : -1) * totalChunkSize;
        }
        else // 수직 이동이 더 큼
        {
            offset.y = (diffY > 0 ? 1 : -1) * totalChunkSize;
        }

        transform.position += offset;

        if (BattleSceneManager.Instance)
            BattleSceneManager.Instance.RecalculatePath();
    }
}
