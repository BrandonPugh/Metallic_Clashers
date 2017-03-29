namespace StageSelection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CustomInput.Information;

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [Serializable]
    public class Tree
    {
        public List<Node> nodes = new List<Node>();
    }

    public class StageSelectionManager : SubManager<StageSelectionManager>
    {
        private List<Tree> m_Worlds = new List<Tree>();

        private int m_Currentworld;
        private Node m_CurrentNode;
        [SerializeField]
        private Text m_StageNameText;
        [SerializeField]
        private Text m_EnemyText;
        [SerializeField]
        private Button m_StartComabtButton;

        public UnityEvent onStageSelectionEnd = new UnityEvent();

        public GameObject nodePrefab;
        public GameObject linePrefab;

        public float spacingMagnitude;



        protected override void Init()
        {
            m_Worlds =
                new List<Tree>
                {
                    new Tree
                    {
                        nodes = new List<Node>
                        {
                            new Node {stageNumber = "1", normalizedPosition = new Vector2(0, 0),
                                worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2", normalizedPosition = new Vector2(0, 1),
                                worldIndex = 1, enemyInts = new List<int> {1,1,1}},
                            new Node {stageNumber = "3", normalizedPosition = new Vector2(0, 2),
                                worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "Boss", normalizedPosition = new Vector2(0, 3),
                                worldIndex = 1, enemyInts = new List<int> {0}},
                            new Node {stageNumber = "1A", normalizedPosition = new Vector2(1, 0),
                                worldIndex = 1, enemyInts = new List<int> {0}},
                            new Node {stageNumber = "2A", normalizedPosition = new Vector2(-1, 1),
                                worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2B", normalizedPosition = new Vector2(-2, 1),
                                worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3A", normalizedPosition = new Vector2(1, 2),
                                worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                        }
                    }
                    ,

                    new Tree
                    {
                        nodes = new List<Node>
                        {
                            new Node {stageNumber = "1", normalizedPosition = new Vector2(0, 0),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2", normalizedPosition = new Vector2(0, 1),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3", normalizedPosition = new Vector2(0, 2),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "Boss", normalizedPosition = new Vector2(0, 3),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2A", normalizedPosition = new Vector2(-1, 1),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3A", normalizedPosition = new Vector2(-1, 2),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2B", normalizedPosition = new Vector2(1, 1),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3B", normalizedPosition = new Vector2(1, 2),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                        }
                    }
                };

            // Make Links

            // Tree 1
            LinkNodes(m_Worlds[0].nodes[0], m_Worlds[0].nodes[1]);
            LinkNodes(m_Worlds[0].nodes[1], m_Worlds[0].nodes[2]);
            LinkNodes(m_Worlds[0].nodes[2], m_Worlds[0].nodes[3]);
            LinkNodes(m_Worlds[0].nodes[0], m_Worlds[0].nodes[4]);
            LinkNodes(m_Worlds[0].nodes[1], m_Worlds[0].nodes[5]);
            LinkNodes(m_Worlds[0].nodes[5], m_Worlds[0].nodes[6]);
            LinkNodes(m_Worlds[0].nodes[2], m_Worlds[0].nodes[7]);

            //Link
            LinkNodes(m_Worlds[0].nodes[3], m_Worlds[1].nodes[0]);

            // Tree 2
            LinkNodes(m_Worlds[1].nodes[0], m_Worlds[1].nodes[1]);
            LinkNodes(m_Worlds[1].nodes[1], m_Worlds[1].nodes[2]);
            LinkNodes(m_Worlds[1].nodes[2], m_Worlds[1].nodes[3]);
            LinkNodes(m_Worlds[1].nodes[1], m_Worlds[1].nodes[4]);
            LinkNodes(m_Worlds[1].nodes[4], m_Worlds[1].nodes[5]);
            LinkNodes(m_Worlds[1].nodes[1], m_Worlds[1].nodes[6]);
            LinkNodes(m_Worlds[1].nodes[6], m_Worlds[1].nodes[7]);

            var canvas = FindObjectOfType<Canvas>();
            var treePos = Vector2.zero;

            //Make Node GameObjects.
            foreach (var tree in m_Worlds)
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
                        new Vector2(n.normalizedPosition.x*spacingMagnitude, 
                        n.normalizedPosition.y*spacingMagnitude) +
                        treePos;

                    var button = nodeObject.GetComponent<Button>();
                    button.onClick.AddListener(() => { SetCurrentNode(n); });
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
                GameManager.self.playerData.worldData = m_Worlds;
            }

            else
            {
                for (var i = 0; i < savedData.Count; i++)
                {
                    for (var j = 0; j < m_Worlds[i].nodes.Count; j++)
                    {
                        m_Worlds[i].nodes[j].isComplete = savedData[i].nodes[j].isComplete;
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

                    lineObject.GetComponent<Image>().color = (monoNode.node.isComplete) 
                        ? Color.blue : Color.red;
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

            //Set Up UI
            m_StageNameText.text = "";
            m_EnemyText.text = "";
            m_StartComabtButton.onClick.AddListener(OnStageSelectionEnd);
            m_StartComabtButton.interactable = false;
        }

        private void OnStageSelectionEnd()
        {
            GameManager.self.enemyIndexes = m_CurrentNode.enemyInts;
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
                m_Currentworld++;
            }
            else if (slideMag.x > 0)
            {
                m_Currentworld--;
            }

            // If we can move, we will as log as the index is 0 to last element in worlds
            if (m_Currentworld >= 0 && m_Currentworld <= m_Worlds.Count - 1)
            {
                foreach (var child in FindObjectsOfType<RectTransform>())
                {
                    if (child.GetComponent<MonoNode>() || child.name.Contains("Line"))
                    {
                        StartCoroutine(
                            MoveObject(child, child.anchoredPosition, 
                            child.anchoredPosition + slideMag, .2f));
                    }
                }
            }
            // If we can't move, Reset index
            else
            {
                if (m_Currentworld >= m_Worlds.Count)
                {
                    m_Currentworld = m_Worlds.Count - 1;
                }
                else if (m_Currentworld < 0)
                {
                    m_Currentworld = 0;
                }
            }
        }

        private IEnumerator MoveObject(RectTransform rt, Vector2 start, Vector2 end, float time)
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

        private void SetCurrentNode(Node n)
        {
            m_CurrentNode = n;
            DisplayInformation();
        }

        private void DisplayInformation()
        {
            if (m_CurrentNode.isComplete)
            {
                m_StageNameText.text = "Stage: " + m_CurrentNode.stageName + " - Completed";
            }
            else
            {
                m_StageNameText.text = "Stage: " + m_CurrentNode.stageName + " - Playable";
            }

            // Enemy Display - Text
            m_EnemyText.text = "";

            var enemyInstances = new Dictionary<string, int>();
            foreach (var index in m_CurrentNode.enemyInts)
            {
                var name = GameManager.self.enemyPrefabList[index].name;
                if(!enemyInstances.ContainsKey(name))
                    enemyInstances.Add(name, 0);
                enemyInstances[name] += 1;
            }

            foreach (var key in enemyInstances)
            {
                m_EnemyText.text += key.Key + " x" + key.Value;
            }

            m_StartComabtButton.interactable = true;
        }
    }
}
