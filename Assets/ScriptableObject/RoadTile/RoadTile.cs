using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoadTile : Tile
{
    public Sprite[] m_Sprites;
    public Sprite m_Preview;
    // ќбновл€ет все тайлы которые напр€мую или по диагонали соприкосаютс€ с текущим
    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (HasRoadTile(tilemap, position))
                    tilemap.RefreshTile(position);
            }
    }
    // Ёто определ€ет, какой спрайт используетс€ на основе RoadTile, которые наход€тс€ р€дом с ним, и поворачивает его, чтобы он соответствовал другим тайлам.
    // ѕоскольку поворот определ€етс€ RoadTile, дл€ плитки устанавливаетс€ TileFlags.OverrideTransform.
    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        int mask = HasRoadTile(tilemap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
        mask += HasRoadTile(tilemap, location + new Vector3Int(1, 0, 0)) ? 2 : 0;
        mask += HasRoadTile(tilemap, location + new Vector3Int(0, -1, 0)) ? 4 : 0;
        mask += HasRoadTile(tilemap, location + new Vector3Int(-1, 0, 0)) ? 8 : 0;
        int index = GetIndex((byte)mask);
        if (index >= 0 && index < m_Sprites.Length)
        {
            tileData.sprite = m_Sprites[index];
            tileData.color = Color.white;
            var m = tileData.transform;
            m.SetTRS(Vector3.zero, GetRotation((byte)mask), Vector3.one);
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.None;
        }
        else
        {
            Debug.LogWarning("Ќедостаточно спрайтов в экземпл€ре RoadTile");
        }
    }

    // ќпредел€ет, €вл€етс€ ли тайл в позиции RoadTile.
    private bool HasRoadTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }
    // —ледующее определ€ет, какой спрайт использовать, исход€ из количества смежных RoadTiles.
    private int GetIndex(byte mask)
    {
        switch (mask)
        {
            case 0: return 0;
            case 3:
            case 6:
            case 9:
            case 12: return 1;
            case 1:
            case 2:
            case 4:
            case 5:
            case 10:
            case 8: return 2;
            case 7:
            case 11:
            case 13:
            case 14: return 3;
            case 15: return 4;
        }
        return -1;
    }
    // —ледующее определ€ет, какой спрайт использовать, исход€ из количества смежных RoadTile.
    private Quaternion GetRotation(byte mask)
    {
        switch (mask)
        {
            case 9:
            case 10:
            case 7:
            case 2:
            case 8:
                return Quaternion.Euler(0f, 0f, -90f);
            case 3:
            case 14:
                return Quaternion.Euler(0f, 0f, -180f);
            case 6:
            case 13:
                return Quaternion.Euler(0f, 0f, -270f);
        }
        return Quaternion.Euler(0f, 0f, 0f);
    }
#if UNITY_EDITOR
// Ќиже приведен вспомогательный код, который добавл€ет пункт меню дл€ создани€ RoadTile Asset.
    [MenuItem("Assets/Create/RoadTile")]
    public static void CreateRoadTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Road Tile", "New Road Tile", "Asset", "Save Road Tile", "Assets");
        if (path == "")
            return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RoadTile>(), path);
    }
#endif
}