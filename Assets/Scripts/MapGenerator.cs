using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public TileBase[] innerTiles;
    public TileBase borderTile;
    public Tilemap tilemap;

    const int mapSize = 32;
    const int minRoomSize = 4;
    const float minDivideRatio = 0.3f;
    const int maxNodeLevel = 4;

    private int[,] map;
    Node rootNode;

    private class Node
    {
        public int left, right;
        public int top, bottom;
        public int width, height;
        public Node leftNode, rightNode;
        public Room room;

        public Node(int l, int r, int t, int b)
        {
            left = l;
            right = r;
            top = t;
            bottom = b;
            width = r - l;
            height = b - t;
        }

    }

    private class Room
    {
        public int left, right;
        public int top, bottom;
        public int width, height;

        public Room(int l, int t, int w, int h)
        {
            left = l;
            right = l + w;
            top = t;
            bottom = t + h;
            width = w;
            height = h;
        }
    }

    private void Awake()
    {
        if (innerTiles.Length != 2)
        {
            Debug.LogErrorFormat("innerTiles.Length should be 2, but it is {0}", innerTiles.Length);
        }
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap();
        }
#endif
    }

    private void GenerateMap()
    {
        map = new int[mapSize, mapSize];
        rootNode = new Node(0, mapSize - 1, 0, mapSize - 1);
        DivideNode(rootNode, 0);
        GenerateRoad(rootNode);

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (map[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y), innerTiles[(x + y) % 2]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y), borderTile);
                }
            }
        }
    }

    // ????怨몄몵嚥?Node ?브쑵釉?
    // Node ??由겼첎? ?臾믪몵筌??브쑵釉??? ??꾪?Room ??밴쉐
    private void DivideNode(Node node, int level)
    {
        // Node ??由겼첎? ?臾믪몵筌??브쑵釉??? ??꾪?Room ??밴쉐
        if (level >= maxNodeLevel || Mathf.Max(node.width, node.height) < (minRoomSize + 1) * 3)
        {
            int roomWidth = Random.Range(minRoomSize, node.right - node.left);
            int roomHeight = Random.Range(minRoomSize, node.bottom - node.top);
            int roomLeft = Random.Range(node.left + 1, node.right - roomWidth);
            int roomTop = Random.Range(node.top + 1, node.bottom - roomHeight);
            node.room = new Room(roomLeft, roomTop, roomWidth, roomHeight);
            OnGenerateRoom(node.room);
            return;
        }

        // ??疫?獄쎻뫚堉???브쑵釉?
        // 揶쎛嚥? ?紐껋쨮 ??由겼첎? 揶쏆늿?앾쭖???뺣쑁??獄쎻뫚堉?
        bool divideDirection = node.height == node.width ? Random.value > 0.5f : node.height > node.width;

        // ?브쑵釉?
        float ratio = Random.Range(minDivideRatio, 1 - minDivideRatio);
        if (divideDirection)
        {
            // ?怨밸릭 ?브쑵釉?
            int middle = node.top + Mathf.RoundToInt(node.height * ratio);
            middle = Mathf.Clamp(middle, node.top + minRoomSize + 1, node.bottom - minRoomSize - 1);
            node.leftNode = new Node(node.left, node.right, node.top, middle);
            node.rightNode = new Node(node.left, node.right, middle, node.bottom);
        }
        else
        {
            // ?ル슣???브쑵釉?
            int middle = node.left + Mathf.RoundToInt(node.width * ratio);
            middle = Mathf.Clamp(middle, node.left + minRoomSize + 1, node.right - minRoomSize - 1);
            node.leftNode = new Node(node.left, middle, node.top, node.bottom);
            node.rightNode = new Node(middle, node.right, node.top, node.bottom);
        }

        // ????怨몄몵嚥??브쑵釉?
        DivideNode(node.leftNode, level + 1);
        DivideNode(node.rightNode, level + 1);
    }

    private void GenerateRoad(Node node)
    {
        // leftNode, rightNode揶쎛 ??용뮉 野껋럩??
        if (node.leftNode == null || node.rightNode == null) { return; }

        Room leftRoom = GetRoom(node.leftNode);
        Room rightRoom = GetRoom(node.rightNode);

        int leftX = Random.Range(leftRoom.left, leftRoom.right);
        int leftY = Random.Range(leftRoom.top, leftRoom.bottom);
        int rightX = Random.Range(rightRoom.left, rightRoom.right);
        int rightY = Random.Range(rightRoom.top, rightRoom.bottom);

        // leftX < rightX??癰귣똻??
        if (leftX > rightX)
        {
            (leftX, rightX) = (rightX, leftX);
            (leftY, rightY) = (rightY, leftY);
        }

        // 疫???밴쉐
        bool direction = Random.value > 0.5; // ?곗뼚??疫뀀챷??獄쎻뫚堉?野껉퀣??
        for (int x = leftX; x <= rightX; x++)
        {
            int y = direction ? leftY : rightY;
            map[x, y] = 1;
        }
        for (int y = Mathf.Min(leftY, rightY); y <= Mathf.Max(leftY, rightY); y++)
        {
            int x = direction ? rightX : leftX;
            map[x, y] = 1;
        }

        // ????怨몄몵嚥?疫???밴쉐
        GenerateRoad(node.leftNode);
        GenerateRoad(node.rightNode);
    }

    private Room GetRoom(Node node)
    {
        // Node揶쎛 Room????釉??野껋럩??
        if (node.room != null) { return node.room; }

        // ??뺣쑁??띿쓺 leftNode, rightNode 餓???롪돌??Room 獄쏆꼹??
        if (Random.value > 0.5f)
        {
            return GetRoom(node.leftNode);
        }
        else
        {
            return GetRoom(node.rightNode);
        }
    }

    private void OnGenerateRoom(Room room)
    {
        for (int x = room.left; x < room.right; x++)
        {
            for (int y = room.top; y < room.bottom; y++)
            {
                map[x, y] = 1;
            }
        }
    }
}
