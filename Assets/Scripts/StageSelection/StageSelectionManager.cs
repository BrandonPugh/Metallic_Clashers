namespace StageSelection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CustomInput.Information;

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [Serializable]
    public class Tree
    {
        public List<Node> nodes = new List<Node>();
    }

    public class StageSelectionManager : SubManager<StageSelectionManager>
    {
        public UnityEvent onStageSelectionEnd = new UnityEvent();

        public GameObject nodePrefab;
        public GameObject linePrefab;
        public Material lineRenderMaterial;
        [HideInInspector]
        public List<Tree> worlds = new List<Tree>();
        public float spacingMagnitude;

        public int currentworld;

        protected override void Init()
        {
            worlds =
                new List<Tree>
                {
                    new Tree
                    {
                        nodes = new List<Node>
                        {
                            new Node {stageNumber = "1", normalizedPosition = new Vector2(0, 0), worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2", normalizedPosition = new Vector2(0, 1), worldIndex = 1, enemyInts = new List<int> {1,1,1}},
                            new Node {stageNumber = "3", normalizedPosition = new Vector2(0, 2), worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "Boss", normalizedPosition = new Vector2(0, 3), worldIndex = 1, enemyInts = new List<int> {0}},
                            new Node {stageNumber = "1A", normalizedPosition = new Vector2(1, 0), worldIndex = 1, enemyInts = new List<int> {0}},
                            new Node {stageNumber = "2A", normalizedPosition = new Vector2(-1, 1), worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2B", normalizedPosition = new Vector2(-2, 1), worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3A", normalizedPosition = new Vector2(1, 2), worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                        }
                    }
                    ,

                    new Tree
                    {
                        nodes = new List<Node>
                        {
                            new Node {stageNumber = "1", normalizedPosition = new Vector2(0, 0), worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2", normalizedPosition = new Vector2(0, 1), worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3", normalizedPosition = new Vector2(0, 2), worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "Boss", normalizedPosition = new Vector2(0, 3), worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2A", normalizedPosition = new Vector2(-1, 1), worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3A", normalizedPosition = new Vector2(-1, 2), worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2B", normalizedPosition = new Vector2(1, 1), worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3B", normalizedPosition = new Vector2(1, 2), worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                        }
                    }
                };

            // Make Links

            // Tree 1
            LinkNodes(worlds[0].nodes[0], worlds[0].nodes[1]);
            LinkNodes(worlds[0].nodes[1], worlds[0].nodes[2]);
            LinkNodes(worlds[0].nodes[2], worlds[0].nodes[3]);
            LinkNodes(worlds[0].nodes[0], worlds[0].nodes[4]);
            LinkNodes(worlds[0].nodes[1], worlds[0].nodes[5]);
            LinkNodes(worlds[0].nodes[5], worlds[0].nodes[6]);
            LinkNodes(worlds[0].nodes[2], worlds[0].nodes[7]);

            //Link
            LinkNodes(worlds[0].nodes[3], worlds[1].nodes[0]);

            // Tree 2
            LinkNodes(worlds[1].nodes[0], worlds[1].nodes[1]);
            LinkNodes(worlds[1].nodes[1], worlds[1].nodes[2]);
            LinkNodes(worlds[1].nodes[2], worlds[1].nodes[3]);
            LinkNodes(worlds[1].nodes[1], worlds[1].nodes[4]);
            LinkNodes(worlds[1].nodes[4], worlds[1].nodes[5]);
            LinkNodes(worlds[1].nodes[1], worlds[1].nodes[6]);
            LinkNodes(worlds[1].nodes[6], worlds[1].nodes[7]);

            var canvas = FindObjectOfType<Canvas>();
            var treePos = Vector2.zero;

            //Make Node GameObjects.
            foreach (var tree in worlds)
            {
                var nodeGameObjects = new List<GameObject>();
                foreach (var n in tree.nodes)
                {

                    var nodeObject = Instantiate(nodePrefab);

                    var monoNode = nodeObject.AddComponent<MonoNode>();
                    monoNode.node = n;

                    nodeObject.transform.SetParent(canvas.transform);

                    var nodeTransform = nodeObject.GetComponent<RectTransform>();
                    nodeTransform.anchoredPosition =
                        new Vector2(n.normalizedPosition.x*spacingMagnitude, n.normalizedPosition.y*spacingMagnitude) +
                        treePos;

                    var button = nodeObject.GetComponent<Button>();
                    button.onClick.AddListener(this.OnStageSelectionEnd);
                    nodeGameObjects.Add(nodeObject);
                }

                // Move to center
                var sum = Vector2.zero;
                var numOfObjects = 0;
                foreach (var node in nodeGameObjects)
                {
                    sum += node.GetComponent<RectTransform>().anchoredPosition;
                    numOfObjects++;
                }

                var offset = sum/numOfObjects;

                foreach (var node in nodeGameObjects)
                {
                    node.GetComponent<RectTransform>().anchoredPosition += treePos - offset;
                }

                treePos.x += 1000;
            }

            // Get Player Data
            var savedData = GameManager.self.playerData.worldData;

            if (savedData.Count == 0)
            {
                GameManager.self.playerData.worldData = worlds;
            }

            else
            {
                for (var i = 0; i < savedData.Count; i++)
                {
                    for (var j = 0; j < worlds[i].nodes.Count; j++)
                    {
                        worlds[i].nodes[j].isComplete = savedData[i].nodes[j].isComplete;
                    }
                }
            }

            // Line Renderers
            var counter = 0;
            foreach (var monoNode in FindObjectsOfType<MonoNode>())
            {
                foreach (var n in monoNode.node.nextNodes)
                {
                    if(n.worldIndex != monoNode.node.worldIndex)
                        continue;

                    var lineObject = Instantiate(linePrefab); //Create GameObject for LineRenderer
                    var lineTransform = lineObject.GetComponent<RectTransform>();
                    lineObject.name = "Line " + counter;
                    counter++;
                    lineObject.transform.SetParent(canvas.transform);
                    GameObject nextNodeGameObject = null;
                    foreach (var g in FindObjectsOfType<MonoNode>())
                    {
                        if (g.GetComponent<MonoNode>().node == n)
                            nextNodeGameObject = g.gameObject;
                    }

                    if (nextNodeGameObject == null)
                        return;

                    var differance = nextNodeGameObject.transform.position - monoNode.transform.position;

                    var linePosition = monoNode.transform.position + (differance/2f);

                    lineTransform.sizeDelta = Math.Abs(differance.x) > Math.Abs(differance.y)
                        ? new Vector2(differance.magnitude, 10)
                        : new Vector2(10, differance.magnitude);

                    lineObject.GetComponent<Image>().color = (monoNode.node.isComplete) ? Color.blue : Color.red;
                        // Set Material Color

                    lineTransform.position = linePosition;
                }
            }

            var startingIndex = GameObject.Find("Line " + (counter - 1)).transform.GetSiblingIndex();

            counter = startingIndex + 1;

            //Move Lines Behind Nodes
            foreach (var m in FindObjectsOfType<RectTransform>())
            {
                if (!m.gameObject.GetComponent<MonoNode>())
                    continue;

                m.transform.SetSiblingIndex(counter);
                counter++;
            }
        }

        public void OnStageSelectionEnd()
        {
            var node = EventSystem.current.currentSelectedGameObject.GetComponent<MonoNode>().node;

            GameManager.self.enemyIndexes = node.enemyInts;
            onStageSelectionEnd.Invoke();
        }

        private void LinkNodes(Node n1, Node n2)
        {
            n1.nextNodes.Add(n2);
            n2.prevNodes.Add(n1);
        }

        protected override void OnEndDrag(DragInformation dragInfo)
        {
            // Which way was it dragged
            var direction = dragInfo.end - dragInfo.origin;

            // Set which way to move
            var slideMag = (direction.x > 0) ? new Vector2(1000, 0) : new Vector2(-1000, 0);

            // Change world Index
            if (slideMag.x < 0)
            {
                currentworld++;
            }
            else if (slideMag.x > 0)
            {
                currentworld--;
            }

            // If we can move, we will as log as the index is 0 to last element in worlds
            if (currentworld >= 0 && currentworld <= worlds.Count - 1)
            {
                foreach (var child in FindObjectsOfType<RectTransform>())
                {
                    if (child.GetComponent<MonoNode>() || child.name.Contains("Line"))
                    {
                        StartCoroutine(
                            MoveObject(child, child.anchoredPosition, child.anchoredPosition + slideMag, .2f));
                    }
                }
            }
            // If we can't move, Reset index
            else
            {
                if (currentworld >= worlds.Count)
                {
                    currentworld = worlds.Count - 1;
                }
                else if (currentworld < 0)
                {
                    currentworld = 0;
                }
            }
        }

        public IEnumerator MoveObject(RectTransform rt, Vector2 start, Vector2 end, float time)
        {
            var i = 0.0;
            var rate = 1.0/time;
            while (i < 1.0)
            {
                i += Time.deltaTime*rate;
                rt.anchoredPosition = Vector3.Lerp(start, end, (float)i);
                yield return null;
            }
        }
    }
}
