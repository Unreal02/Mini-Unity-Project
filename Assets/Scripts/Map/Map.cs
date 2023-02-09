using UnityEngine;
using UnityEngine.Assertions;

public class MapTile
{
    public enum TileType { Wall, Ground }

    public TileType type;
    public Character character;
    public int roomId;

    public MapTile() { type = TileType.Wall; }

    public bool IsMovable()
    {
        return type == TileType.Ground && character == null;
    }
}

public class Map
{
    const int mapSize = 32;
    const int minRoomSize = 4;
    const float minDivideRatio = 0.3f;
    const int maxNodeLevel = 4;

    MapTile[,] map;
    Node rootNode;
    private int maxRoomId = 1;

    private class Node
    {
        public int left, right;
        public int bottom, top;
        public int width, height;
        public Node leftNode, rightNode;
        public Room room;
        public int roomCount;

        public Node(int l, int r, int b, int t)
        {
            left = l;
            right = r;
            bottom = b;
            top = t;
            width = r - l;
            height = t - b;
            roomCount = 0;
        }

        public void UpdateRoomCount()
        {
            roomCount = 0;
            if (room != null) { roomCount++; }
            if (leftNode != null)
            {
                leftNode.UpdateRoomCount();
                roomCount += leftNode.roomCount;
            }
            if (rightNode != null)
            {
                rightNode.UpdateRoomCount();
                roomCount += rightNode.roomCount;
            }
        }
    }

    private class Room
    {
        public int left, right;
        public int bottom, top;
        public int width, height;
        public int id;

        public Room(int l, int b, int w, int h, int i)
        {
            left = l;
            right = l + w;
            bottom = b;
            top = b + h;
            width = w;
            height = h;
            id = i;
        }
    }

    public Map()
    {
        map = new MapTile[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                map[i, j] = new MapTile();
            }
        }
        maxRoomId = 1;
        rootNode = new Node(0, mapSize - 1, 0, mapSize - 1);
        DivideNode(rootNode, 0);
        GenerateRoad(rootNode);
        rootNode.UpdateRoomCount();
        Assert.AreEqual(maxRoomId - 1, rootNode.roomCount);
    }

    public int GetSize() { return mapSize; }

    public MapTile GetTile(int x, int y)
    {
        if (x < 0 || x >= mapSize || y < 0 || y >= mapSize) return null;
        return map[x, y];
    }

    public (int, int) GetRandomPosition()
    {
        Room room = GetRandomRoom(rootNode);
        int x = Random.Range(room.left, room.right);
        int y = Random.Range(room.bottom, room.top);
        return (x, y);
    }

    // 재귀적으로 Node 분할
    // Node 크기가 작으면 분할하지 않고 Room 생성
    private void DivideNode(Node node, int level)
    {
        // Node 크기가 작으면 분할하지 않고 Room 생성
        if (level >= maxNodeLevel || Mathf.Max(node.width, node.height) < (minRoomSize + 1) * 3)
        {
            int roomWidth = Random.Range(minRoomSize, node.right - node.left);
            int roomHeight = Random.Range(minRoomSize, node.top - node.bottom);
            int roomLeft = Random.Range(node.left + 1, node.right - roomWidth);
            int roomTop = Random.Range(node.bottom + 1, node.top - roomHeight);
            node.room = new Room(roomLeft, roomTop, roomWidth, roomHeight, maxRoomId++);
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
            int middle = node.bottom + Mathf.RoundToInt(node.height * ratio);
            middle = Mathf.Clamp(middle, node.bottom + minRoomSize + 1, node.top - minRoomSize - 1);
            node.leftNode = new Node(node.left, node.right, node.bottom, middle);
            node.rightNode = new Node(node.left, node.right, middle, node.top);
        }
        else
        {
            // 좌우 분할
            int middle = node.left + Mathf.RoundToInt(node.width * ratio);
            middle = Mathf.Clamp(middle, node.left + minRoomSize + 1, node.right - minRoomSize - 1);
            node.leftNode = new Node(node.left, middle, node.bottom, node.top);
            node.rightNode = new Node(middle, node.right, node.bottom, node.top);
        }

        // 재귀적으로 분할
        DivideNode(node.leftNode, level + 1);
        DivideNode(node.rightNode, level + 1);
    }

    private void GenerateRoad(Node node)
    {
        // leftNode, rightNode가 없는 경우
        if (node.leftNode == null || node.rightNode == null) { return; }

        Room leftRoom = GetRandomRoom(node.leftNode);
        Room rightRoom = GetRandomRoom(node.rightNode);

        int leftX = Random.Range(leftRoom.left, leftRoom.right);
        int leftY = Random.Range(leftRoom.bottom, leftRoom.top);
        int rightX = Random.Range(rightRoom.left, rightRoom.right);
        int rightY = Random.Range(rightRoom.bottom, rightRoom.top);

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
            map[x, y].type = MapTile.TileType.Ground;
        }
        for (int y = Mathf.Min(leftY, rightY); y <= Mathf.Max(leftY, rightY); y++)
        {
            int x = direction ? rightX : leftX;
            map[x, y].type = MapTile.TileType.Ground;
        }

        // 재귀적으로 길 생성
        GenerateRoad(node.leftNode);
        GenerateRoad(node.rightNode);
    }

    private Room GetRandomRoom(Node node)
    {
        // Node가 Room을 포함한 경우
        if (node.room != null) { return node.room; }

        // 랜덤하게 leftNode, rightNode 중 하나의 Room 반환 (방 개수만큼 가중치)
        int leftRoomCount = node.leftNode.roomCount;
        int rightRoomCount = node.rightNode.roomCount;
        if (Random.Range(0, leftRoomCount + rightRoomCount) < leftRoomCount)
        {
            return GetRandomRoom(node.leftNode);
        }
        else
        {
            return GetRandomRoom(node.rightNode);
        }
    }

    private void OnGenerateRoom(Room room)
    {
        for (int x = room.left; x < room.right; x++)
        {
            for (int y = room.bottom; y < room.top; y++)
            {
                map[x, y].type = MapTile.TileType.Ground;
                map[x, y].roomId = room.id;
            }
        }
    }
}
