using UnityEngine;
using UnityEngine.Assertions;

public class MapTile
{
    public enum TileType { Wall, Ground, Goal }

    public TileType type;
    public Character character;
    public int roomId;

    public MapTile() { type = TileType.Wall; }

    public bool IsMovable()
    {
        return type != TileType.Wall && character == null;
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
        this.GenerateGoal();
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

    // ??????????????? Node ??????
    // Node ????????? ????????? ???????????? ?????? Room ??????
    private void DivideNode(Node node, int level)
    {
        // Node ????????? ????????? ???????????? ?????? Room ??????
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

        // ??? ??? ????????? ??????
        // ??????, ?????? ????????? ????????? ????????? ??????
        bool divideDirection = node.height == node.width ? Random.value > 0.5f : node.height > node.width;

        // ??????
        float ratio = Random.Range(minDivideRatio, 1 - minDivideRatio);
        if (divideDirection)
        {
            // ?????? ??????
            int middle = node.bottom + Mathf.RoundToInt(node.height * ratio);
            middle = Mathf.Clamp(middle, node.bottom + minRoomSize + 1, node.top - minRoomSize - 1);
            node.leftNode = new Node(node.left, node.right, node.bottom, middle);
            node.rightNode = new Node(node.left, node.right, middle, node.top);
        }
        else
        {
            // ?????? ??????
            int middle = node.left + Mathf.RoundToInt(node.width * ratio);
            middle = Mathf.Clamp(middle, node.left + minRoomSize + 1, node.right - minRoomSize - 1);
            node.leftNode = new Node(node.left, middle, node.bottom, node.top);
            node.rightNode = new Node(middle, node.right, node.bottom, node.top);
        }

        // ??????????????? ??????
        DivideNode(node.leftNode, level + 1);
        DivideNode(node.rightNode, level + 1);
    }

    private void GenerateRoad(Node node)
    {
        // leftNode, rightNode??? ?????? ??????
        if (node.leftNode == null || node.rightNode == null) { return; }

        Room leftRoom = GetRandomRoom(node.leftNode);
        Room rightRoom = GetRandomRoom(node.rightNode);

        int leftX = Random.Range(leftRoom.left, leftRoom.right);
        int leftY = Random.Range(leftRoom.bottom, leftRoom.top);
        int rightX = Random.Range(rightRoom.left, rightRoom.right);
        int rightY = Random.Range(rightRoom.bottom, rightRoom.top);

        // leftX < rightX??? ??????
        if (leftX > rightX)
        {
            (leftX, rightX) = (rightX, leftX);
            (leftY, rightY) = (rightY, leftY);
        }

        // ??? ??????
        bool direction = Random.value > 0.5; // ?????? ?????? ?????? ??????
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

        // ??????????????? ??? ??????
        GenerateRoad(node.leftNode);
        GenerateRoad(node.rightNode);
    }

    private void GenerateGoal()
    {
        (int x, int y) = this.GetRandomPosition();
        this.GetTile(x, y).type = MapTile.TileType.Goal;
    }

    private Room GetRandomRoom(Node node)
    {
        // Node??? Room??? ????????? ??????
        if (node.room != null) { return node.room; }

        // ???????????? leftNode, rightNode ??? ????????? Room ?????? (??? ???????????? ?????????)
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
