using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour
{

    public Tilemap obstaclesTileMap;
    public TileBase obstacleTileDebug;
    public TileBase navegableTileDebug;
    Node[,]grid;
    Dictionary<Vector3Int, Node> gridDistionary = new Dictionary<Vector3Int, Node>();


    private void Start(){
        CreateGrid();
    }

    private void Update(){

    }

    void CreateGrid(){
        grid = new Node[obstaclesTileMap.cellBounds.size.x, obstaclesTileMap.cellBounds.size.y];

        int xt = obstaclesTileMap.cellBounds.xMin;
        for (int x = 0; x < obstaclesTileMap.cellBounds.size.x; x++){
            
            int yt = obstaclesTileMap.cellBounds.yMin;

            for (int y = 0; y < obstaclesTileMap.cellBounds.size.y; y++){
                bool isPath = true;

                TileBase tile = obstaclesTileMap.GetTilesBlock(obstaclesTileMap.cellBounds)[x + y * obstaclesTileMap.cellBounds.size.x];

                if (tile != null){
                    isPath = false;
                }

                grid[x,y] = new Node(isPath, obstaclesTileMap.CellToWorld(new Vector3Int(xt, yt, 0)), x,y, new Vector3Int(xt,yt,0));
                gridDistionary.Add(new Vector3Int(xt, yt, 0), grid[x, y]);
                yt++;
            }
            xt++;
        }
    }

    public Node NodeFromWorldPosition(Vector3 WorldPos){
        Vector3Int pt = obstaclesTileMap.WorldToCell(WorldPos);
        return gridDistionary[pt];
    }

    public List<Node> GetNeighborNodes(Node n){
                List<Node> neighborList = new List<Node>();
                int xCheck;
                int yCheck;

                xCheck  = n.gridX+1;
                yCheck = n.gridY;
                if(xCheck >= 0 && xCheck < grid.GetLength(0) && yCheck >= 0 && yCheck < grid.GetLength(1)){
                    neighborList.Add(grid[xCheck,yCheck]);
                }


                xCheck  = n.gridX - 1;
                yCheck = n.gridY;
                if(xCheck >= 0 && xCheck < grid.GetLength(0) && yCheck >= 0 && yCheck < grid.GetLength(1)){
                    neighborList.Add(grid[xCheck,yCheck]);
                }


                xCheck  = n.gridX;
                yCheck = n.gridY + 1;
                if(xCheck >= 0 && xCheck < grid.GetLength(0) && yCheck >= 0 && yCheck < grid.GetLength(1)){
                    neighborList.Add(grid[xCheck,yCheck]);
                }


                xCheck  = n.gridX;
                yCheck = n.gridY - 1;
                if(xCheck >= 0 && xCheck < grid.GetLength(0) && yCheck >= 0 && yCheck < grid.GetLength(1)){
                    neighborList.Add(grid[xCheck,yCheck]);
                }


                xCheck  = n.gridX-1;
                yCheck = n.gridY - 1;
                if(xCheck >= 0 && xCheck < grid.GetLength(0) && yCheck >= 0 && yCheck < grid.GetLength(1)){
                    grid[xCheck,yCheck].secondDiagonal = Mathf.Sqrt(2); // Its a diagonal child so for the heuristic use the octile distance
                    neighborList.Add(grid[xCheck,yCheck]);
                }


                xCheck  = n.gridX+1;
                yCheck = n.gridY - 1;
                if(xCheck >= 0 && xCheck < grid.GetLength(0) && yCheck >= 0 && yCheck < grid.GetLength(1)){
                    grid[xCheck,yCheck].secondDiagonal = Mathf.Sqrt(2); // Its a diagonal child so for the heuristic use the octile distance
                    neighborList.Add(grid[xCheck,yCheck]);
                }


                xCheck  = n.gridX+1;
                yCheck = n.gridY + 1;
                if(xCheck >= 0 && xCheck < grid.GetLength(0) && yCheck >= 0 && yCheck < grid.GetLength(1)){
                    grid[xCheck,yCheck].secondDiagonal = Mathf.Sqrt(2); // Its a diagonal child so for the heuristic use the octile distance
                    neighborList.Add(grid[xCheck,yCheck]);
                }


                xCheck  = n.gridX -1;
                yCheck = n.gridY + 1;
                if(xCheck >= 0 && xCheck < grid.GetLength(0) && yCheck >= 0 && yCheck < grid.GetLength(1)){
                    grid[xCheck,yCheck].secondDiagonal = Mathf.Sqrt(2); // Its a diagonal child so for the heuristic use the octile distance
                    neighborList.Add(grid[xCheck,yCheck]);
                }

        return neighborList;
    }

    public void ResetGrid(){
        foreach (Node n in grid){
            n.gCost = 999999;
            n.hCost = 0;
            n.parent = null;
        }
    }
}
