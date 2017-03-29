using UnityEngine;

namespace Combat
{
    using System.Collections;
    using System.Linq;

    using CustomInput;
    using CustomInput.Information;

    public class TargetingEnemy : MonoBehaviour
    {
        private EnemyMono m_CurrentEnemyMono;
        private IEnumerator m_AnimateCorutine;
        private Vector3 m_OriginalPosition;
        private bool m_Rising = true;

        public Transform marker;

        private void Start ()
        {
            m_AnimateCorutine = Animate();                  // Set up Corutine
            m_OriginalPosition = marker.position;           // Set Markers original position
            m_CurrentEnemyMono = CombatManager.self.currentEnemy;           // Get current enemy
            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
            InputManager.self.onPress.AddListener(OnPress); // Set Listener for input
           
            // Set Marker Position
            var currenyEnemyBounds = m_CurrentEnemyMono.GetComponent<MeshRenderer>().bounds;
            marker.position = currenyEnemyBounds.center +
                new Vector3(0, currenyEnemyBounds.extents.y + .5f, 0);

            // Start Animateing marker
            StartCoroutine(m_AnimateCorutine);
        }

        private void OnCombatUpdate()
        {
            // If the enemy is not null or there are no enemies, return
            if (m_CurrentEnemyMono != null || CombatManager.self.enemies.Count == 0)
                return;

            // If current enemy is null and there are enemies

            m_CurrentEnemyMono = CombatManager.self.enemies.First();    // Find the first guy

            // Set Marker
            var currenyEnemyBounds = m_CurrentEnemyMono.GetComponent<MeshRenderer>().bounds;
            marker.position = currenyEnemyBounds.center +
                new Vector3(0, currenyEnemyBounds.extents.y + .5f, 0);

            // Set new enemy
            CombatManager.self.currentEnemy = m_CurrentEnemyMono;
        }

        private void OnPress(TouchInformation touchInfo)
        {
            if (CombatCamera.isAnimating)   // if the camera is animating, do nothing
                return;

            // Else shoot ray from touch
            var ray = Camera.main.ScreenPointToRay(touchInfo.position);
            var hit = new RaycastHit();

            try
            {
                Physics.Raycast(ray.origin, ray.direction, out hit);
            }
            catch { }

            if (hit.transform == null)  // Did the ray hit something
                return;

            var tempObject = hit.transform.gameObject;  //Store gameobject temperarily.
            var gameOb = tempObject;    // This will be the final result.

            // Keep going until EnemyMono if found or if none were found.
            while(true)
            {
                if (tempObject.GetComponent<EnemyMono>())   // Does it have an EnemyMono
                {
                    gameOb = tempObject;
                    break;
                }
                // If Not, set temp to its parent
                if (tempObject.transform.parent != null 
                    && !tempObject.GetComponent<EnemyMono>())
                    tempObject = tempObject.transform.parent.gameObject;

                // If no parent, reached end, return function.
                else
                {
                    return;
                }
            }
            
            // EnemyMono same as the current target
            if (gameOb == m_CurrentEnemyMono.gameObject)
            {
                // Set position of marker
                var currenyEnemyBounds = m_CurrentEnemyMono.GetComponent<MeshRenderer>().bounds;
                marker.position = currenyEnemyBounds.center +
                    new Vector3(0, currenyEnemyBounds.extents.y + .5f, 0);

                CombatManager.self.currentEnemy = m_CurrentEnemyMono;   // Set enemy

                CombatCamera.isAnimating = true;            // Turn combat Camera back on
            }
            // Clicked a different EnemyMono than the current.
            else if (gameOb.GetComponent<EnemyMono>() != m_CurrentEnemyMono)
            {
                Camera.main.transform.localPosition = new Vector3(0, 0, -5f);
                m_CurrentEnemyMono = gameOb.GetComponent<EnemyMono>();
                marker.position = m_CurrentEnemyMono.transform.position + new Vector3(0, 1f, 0);
            }
        }

        private IEnumerator Animate()
        {
            while (true)
            {
                if (m_Rising)
                {
                    marker.position += new Vector3(0, 2 *Time.deltaTime, 0);
                }

                else
                {
                    marker.position -= new Vector3(0, Time.deltaTime, 0);
                }

                if (marker.position.y > m_OriginalPosition.y + .5f)
                {
                    m_Rising = false;
                }                   

                if (marker.position.y < m_OriginalPosition.y - .5f)
                {
                    m_Rising = true;
                }

                yield return null;
            }
        }
    }
}