using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public Transform tilePrefab;
    public Transform obsPrefab;
    public Transform mapFloor;
    public Transform mask;
    public Transform realMapFloor;
    Transform[,] tileMap;
    public Vector2 maxSize;
    [Range(0, 1)]
    public float outline;
    public float tileSize;
    public Map[] maps;
    public int mapIndex;
    Map currentMap;
    List<coord> allCoords;
    Queue<coord> mixedCoords;
    Queue<coord> mixedTileCoords;

    [System.Serializable]
    public struct coord
    {
        public int x;
        public int y;

        public coord(int newX, int newY)
        {
            x = newX;
            y = newY;
        }

        public static bool operator ==(coord c1, coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(coord c1, coord c2)
        {
            return !(c1 == c2);
        }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override bool Equals(object obj) { return base.Equals(obj); }
    }

    public void generateMap()
    {
        currentMap = maps[mapIndex];
        System.Random psudoRand = new System.Random(currentMap.seed);
        allCoords = new List<coord>();
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];

        //making coords
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allCoords.Add(new coord(x, y));
            }
        }
        //randomizing the coords
        mixedCoords = new Queue<coord>(Utility.shuffleArray(allCoords.ToArray(), currentMap.seed));

        string holderName = "Generated Map";
        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }
        //creating mapHolder
        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for(int x = 0; x < currentMap.mapSize.x; x++)
        {
            for(int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePos = toPosition(x, y);
                Transform newTile = (Transform)Instantiate(tilePrefab, tilePos, Quaternion.Euler(Vector3.right * 90));
                newTile.localScale = Vector3.one * (1 - outline) * tileSize;
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }

        //spawning obsticles
        bool[,] obsMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        int numberOfObsticles = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obsPercent);
        int obsCount = 0;
        List<coord> openCoords = new List<coord>(allCoords);

        for (int i = 0; i < numberOfObsticles; i++)
        {
            coord random = getRandomCoord();
            obsMap[random.x, random.y] = true;
            Vector3 obsPos = toPosition(random.x, random.y);
            obsCount++;
            if(random != currentMap.mapCenter && mapHasPath(obsMap, obsCount))
            {
                float obsHeight = Mathf.Lerp(currentMap.minObsHeight, currentMap.maxObsHeight, (float)psudoRand.NextDouble());
                Transform newObs = (Transform)Instantiate(obsPrefab, obsPos + Vector3.up * (obsHeight / 2), Quaternion.identity);
                newObs.parent = mapHolder;
                newObs.localScale = new Vector3((1 - outline) * tileSize, obsHeight, (1 - outline) * tileSize);

                Renderer obsRend = newObs.GetComponent<Renderer>();
                Material obsMat = new Material(obsRend.sharedMaterial);
                float colorPercent = random.y / (float)currentMap.mapSize.y;
                obsMat.color = Color.Lerp(currentMap.forground, currentMap.background, colorPercent);
                obsRend.sharedMaterial = obsMat;

                openCoords.Remove(random);
            }
            else
            {
                obsMap[random.x, random.y] = false;
                obsCount--;
            }
        }

        mixedTileCoords = new Queue<coord>(Utility.shuffleArray(openCoords.ToArray(), currentMap.seed));

        //creating mask
        Transform maskLeft = Instantiate(mask, Vector3.left * (currentMap.mapSize.x + maxSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(mask, Vector3.right * (currentMap.mapSize.x + maxSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(mask, Vector3.forward * (currentMap.mapSize.y + maxSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxSize.x, 1, (maxSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(mask, Vector3.back * (currentMap.mapSize.y + maxSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxSize.x, 1, (maxSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        mapFloor.localScale = new Vector3(maxSize.x, maxSize.y) * tileSize;
        realMapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
    }

    Vector3 toPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2 + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public coord getRandomCoord()
    {
        coord random = mixedCoords.Dequeue();
        mixedCoords.Enqueue(random);
        return random;
    }

    public Transform getRandomOpenTile()
    {
        coord random = mixedTileCoords.Dequeue();
        mixedTileCoords.Enqueue(random);
        return tileMap[random.x, random.y];
    }

    bool mapHasPath(bool[,] obsMap, int currentCount)
    {
        bool[,] flags = new bool[obsMap.GetLength(0), obsMap.GetLength(1)];
        Queue<coord> queue = new Queue<coord>();
        queue.Enqueue(currentMap.mapCenter);
        flags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;
        int useableTileCount = 1;
        int targetTileCount;

        //flood fill starts here
        while(queue.Count > 0)
        {
            coord tile = queue.Dequeue();
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;
                    if(x == 0 || y == 0)
                    {
                        if(neighborX >= 0 && neighborX < obsMap.GetLength(0) && neighborY >= 0 && neighborY < obsMap.GetLength(1))
                        {
                            if(!flags[neighborX, neighborY] && !obsMap[neighborX, neighborY])
                            {
                                flags[neighborX, neighborY] = true;
                                queue.Enqueue(new coord(neighborX, neighborY));
                                useableTileCount++;
                            }
                        }
                    }
                }
            }
        }

        targetTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentCount);
        return targetTileCount == useableTileCount;
    }

	void Awake () {
        FindObjectOfType<Spawner>().onNewWave += onNewWave;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [System.Serializable]
    public class Map {
        public coord mapSize;
        [Range(0, 1)]
        public float obsPercent;
        public int seed;
        public float minObsHeight;
        public float maxObsHeight;
        public Color forground;
        public Color background;
        public coord mapCenter
        {
            get
            {
                return new coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }

    public Transform getTileFromPostion(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);
        return tileMap[x, y];
    }

    void onNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        generateMap();
    }

}
