using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tile innerTile;
    public Tile borderTile;
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
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap();
        }
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
                    tilemap.SetTile(new Vector3Int(x, y), innerTile);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y), borderTile);
                }
            }
        }
    }

    // 재귀적으로 Node 분할
    // Node 크기가 작으면 분할하지 않고 Room 생성
    private void DivideNode(Node node, int level)
    {
        // Node 크기가 작으면 분할하지 않고 Room 생성
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

        // 더 긴 방향을 분할
        // 가로, 세로 크기가 같으면 랜덤한 방향
        bool divideDirection = node.height == node.width ? Random.value > 0.5f : node.height > node.width;

        // 분할
        float ratio = Random.Range(minDivideRatio, 1 - minDivideRatio);
        if (divideDirection)
        {
            // 상하 분할
            int middle = node.top + Mathf.RoundToInt(node.height * ratio);
            middle = Mathf.Clamp(middle, node.top + minRoomSize + 1, node.bottom - minRoomSize - 1);
            node.leftNode = new Node(node.left, node.right, node.top, middle);
            node.rightNode = new Node(node.left, node.right, middle, node.bottom);
        }
        else
        {
            // 좌우 분할
            int middle = node.left + Mathf.RoundToInt(node.width * ratio);
            middle = Mathf.Clamp(middle, node.left + minRoomSize + 1, node.right - minRoomSize - 1);
            node.leftNode = new Node(node.left, middle, node.top, node.bottom);
            node.rightNode = new Node(middle, node.right, node.top, node.bottom);
        }

        // 재귀적으로 분할
        DivideNode(node.leftNode, level + 1);
        DivideNode(node.rightNode, level + 1);
    }

    private void GenerateRoad(Node node)
    {
        // leftNode, rightNode가 없는 경우
        if (node.leftNode == null || node.rightNode == null) { return; }

        Room leftRoom = GetRoom(node.leftNode);
        Room rightRoom = GetRoom(node.rightNode);

        int leftX = Random.Range(leftRoom.left, leftRoom.right);
        int leftY = Random.Range(leftRoom.top, leftRoom.bottom);
        int rightX = Random.Range(rightRoom.left, rightRoom.right);
        int rightY = Random.Range(rightRoom.top, rightRoom.bottom);

        // leftX < rightX를 보장
        if (leftX > rightX)
        {
            (leftX, rightX) = (rightX, leftX);
            (leftY, rightY) = (rightY, leftY);
        }

        // 길 생성
        bool direction = Random.value > 0.5; // 꺾인 길의 방향 결정
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

        // 재귀적으로 길 생성
        GenerateRoad(node.leftNode);
        GenerateRoad(node.rightNode);
    }

    private Room GetRoom(Node node)
    {
        // Node가 Room을 포함한 경우
        if (node.room != null) { return node.room; }

        // 랜덤하게 leftNode, rightNode 중 하나의 Room 반환
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
