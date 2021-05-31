using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Grid Manager is used to give access to the Tile Layers and check to see if there is something at this layer or to place/replace something on this layer
/// </summary>
public class GridManager : MonoBehaviour
{
    private static GridManager s_gridManager;

    public static GridManager GetGridManager()
    {
        //See if tprivhere is a GridManager available in in the static entry or in the Game World
        if(s_gridManager == null)
        {
            s_gridManager = FindObjectOfType<Grid>().GetComponent<GridManager>();
        }
        //No Grid Manager was found
        if(s_gridManager == null)
        {
            Debug.LogError("No GridManager was found");
            return null;
        }
        return s_gridManager;
    }


    public enum Layers
    {
        k_collectables, k_obstacles, k_background, k_decorations, k_groundDecorations
    }

    private static Dictionary<Layers, string> m_layerToName = new Dictionary<Layers, string>()
    {
        { Layers.k_collectables, "Collectables" },
        { Layers.k_background, "Background" },
        { Layers.k_decorations, "Decorations" },
        { Layers.k_groundDecorations, "GroundDecorations" },
        { Layers.k_obstacles, "Obstacles" }
    };
    //look up for all maps
    private Dictionary<string, Tilemap> m_tileMaps;

    /// <summary>
    /// Set up the maps
    /// </summary>
    private void Start()
    {
        //Add all the maps to the maps list by the 
        var maps = GetComponentsInChildren<Tilemap>();
        {
            foreach(var map in maps)
            {
                m_tileMaps.Add(map.gameObject.name, map);
            }
        }
    }
    /// <summary>
    /// Returns true if the Cell at the specified Position is filled on that layer
    /// </summary>
    /// <param name="Layer">Layer to Check(GameObject Name)</param>
    /// <param name="position">Position to check</param>
    /// <returns></returns>
   public bool GridCellIsFilled(Layers layer, Vector3Int position)
    {
        return m_tileMaps[m_layerToName[layer]].HasTile(position);
    }
    /// <summary>
    /// Overload to check a world position to see if the corresponding Grid cell is filled
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool GridCellIsFilled(Layers layer, Vector3 position)
    {
        return GridCellIsFilled(layer, m_tileMaps[m_layerToName[layer]].WorldToCell(position));
    }



}
