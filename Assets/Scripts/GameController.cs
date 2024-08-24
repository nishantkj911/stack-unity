using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class GameController : MonoBehaviour
{
    public float maxLimitBlock = 2.5f;
    public GameObject blockPrefab;
    public float errorMargin = 0.25f; // TODO: Modify to actual value.
    public int Score { get; private set; } = 0;
    public event EventHandler OnGameOver = delegate { }; // Giving a basic subscriber.

    private float _blockHeight;

    [FormerlySerializedAs("_blockCenter")] [SerializeField]
    private Transform blockCenter;

    private GameObject _currentBlock;
    private EulerAxis _currentEulerAxis = EulerAxis.X; // TODO: Randomize this

    private void MyOnGameOverHandler(object sender, EventArgs e)
    {
        Debug.Log("Game Over!! Score: " + Score);
        _currentBlock.AddComponent<DestroyOnDiscarded>();
    }

    private GameObject[] CutBlock(GameObject currentBlock, Vector3 newScale, Vector3 newBlockCenter,
        Vector3 cutBlockScale,
        Vector3 cutBlockCenter)
    {
        /*
         * Returns: An array with 2 elements, the new block and the cut block with physics enabled.
         */
        if ((newScale.x <= 0 || newScale.z <= 0) || (cutBlockScale.x <= 0 || cutBlockScale.z <= 0))
        {
            var cbRb = currentBlock.GetComponent<Rigidbody>();
            cbRb.useGravity = true;
            cbRb.isKinematic = false;
            throw new Exception("Invalid Block Size"); // TODO: Change the type of exception to be more specific.
        }

        Destroy(currentBlock);

        // TODO: Name the block appropriately to be understandable in the hierarchy
        var newBlock = Instantiate(_currentBlock, newBlockCenter, Quaternion.identity);
        newBlock.transform.localScale = newScale;

        var cutBlock = Instantiate(_currentBlock, cutBlockCenter, Quaternion.identity);
        cutBlock.transform.localScale = cutBlockScale;
        cutBlock.AddComponent<DestroyOnDiscarded>();
        var cutBlockRigidBody = cutBlock.GetComponent<Rigidbody>();
        cutBlockRigidBody.isKinematic = false;
        cutBlockRigidBody.useGravity = true;

        return new[] { newBlock, cutBlock };
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Start is called before the first frame update
    void Awake()
    {
        _blockHeight = blockPrefab.transform.localScale.y;
        OnGameOver += MyOnGameOverHandler;
    }

    void Start()
    {
        _currentBlock = Instantiate(blockPrefab, blockCenter.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

        if (!Input.GetKeyDown(KeyCode.Space)) return;
        // Debug.Log("space key pressed");

        // Stop current block's movement
        if (_currentBlock)
        {
            _currentBlock.GetComponent<BlockMovement>().enabled = false;
        }

        // Calculate distance between the block center and current block.
        var distance = Vector3.Distance(blockCenter.position, _currentBlock.transform.position);
        GameObject newCurrentBlock = null;


        if (distance <= errorMargin) // TODO: Can this be optimized? Can this be simplified?
        {
            // Case of block aligning 
            Debug.Log("Block aligned");
            _currentBlock.transform.position = blockCenter.position;

            newCurrentBlock = _currentBlock;
        }
        else
        {
            // Case of block misaligning.

            // A lot of math here, but basically seeing the distance between the 2 centers and instantiating 2 block with total same dimensions.
            try
            {
                var vectorDistance = blockCenter.position - _currentBlock.transform.position;
                Vector3 newScale, newBlockCenter, cutBlockScale, cutBlockCenter;
                switch (_currentBlock.GetComponent<BlockMovement>().axis)
                {
                    case EulerAxis.X:
                        if (vectorDistance.x > 0) // Don't confuse polarity here. The axis coords are < 0 hence undershot.
                        {
                            Debug.Log("x undershot");
                            newScale = new Vector3(_currentBlock.transform.localScale.x - distance, _blockHeight, _currentBlock.transform.localScale.z);
                            newBlockCenter = new Vector3(_currentBlock.transform.position.x + distance / 2, _currentBlock.transform.position.y, _currentBlock.transform.position.z);

                            cutBlockScale = new Vector3(distance, _blockHeight, _currentBlock.transform.localScale.z);
                            cutBlockCenter = new Vector3(blockCenter.transform.position.x - _currentBlock.transform.localScale.x / 2 - distance / 2, _currentBlock.transform.position.y,
                                _currentBlock.transform.position.z);

                            var slicedBlocks = CutBlock(_currentBlock, newScale, newBlockCenter, cutBlockScale, cutBlockCenter);
                            newCurrentBlock = slicedBlocks[0];
                        }
                        else
                        {
                            Debug.Log("x overshot");
                            newScale = new Vector3(_currentBlock.transform.localScale.x - distance, _blockHeight, _currentBlock.transform.localScale.z);
                            newBlockCenter = new Vector3(_currentBlock.transform.position.x - distance / 2, _currentBlock.transform.position.y, _currentBlock.transform.position.z);

                            cutBlockScale = new Vector3(distance, _blockHeight, _currentBlock.transform.localScale.z);
                            cutBlockCenter = new Vector3(blockCenter.transform.position.x + _currentBlock.transform.localScale.x / 2 + distance / 2, _currentBlock.transform.position.y,
                                _currentBlock.transform.position.z);

                            var slicedBlocks = CutBlock(_currentBlock, newScale, newBlockCenter, cutBlockScale, cutBlockCenter);
                            newCurrentBlock = slicedBlocks[0];
                        }

                        break;
                    case EulerAxis.Z:
                        if (vectorDistance.z > 0) // Don't confuse polarity here. The axis coords are < 0 hence undershot.
                        {
                            Debug.Log("z undershot");
                            newScale = new Vector3(_currentBlock.transform.localScale.x, _blockHeight, _currentBlock.transform.localScale.z - distance);
                            newBlockCenter = new Vector3(_currentBlock.transform.position.x, _currentBlock.transform.position.y, _currentBlock.transform.position.z + distance / 2);

                            cutBlockScale = new Vector3(_currentBlock.transform.localScale.x, _blockHeight, distance);
                            cutBlockCenter = new Vector3(_currentBlock.transform.position.x, _currentBlock.transform.position.y,
                                blockCenter.transform.position.z - _currentBlock.transform.localScale.z / 2 - distance / 2);

                            var slicedBlocks = CutBlock(_currentBlock, newScale, newBlockCenter, cutBlockScale, cutBlockCenter);
                            newCurrentBlock = slicedBlocks[0];
                        }
                        else
                        {
                            Debug.Log("z overshot");
                            newScale = new Vector3(_currentBlock.transform.localScale.x, _blockHeight, _currentBlock.transform.localScale.z - distance);
                            newBlockCenter = new Vector3(_currentBlock.transform.position.x, _currentBlock.transform.position.y, _currentBlock.transform.position.z - distance / 2);

                            cutBlockScale = new Vector3(_currentBlock.transform.localScale.x, _blockHeight, distance);
                            cutBlockCenter = new Vector3(_currentBlock.transform.position.x, _currentBlock.transform.position.y,
                                blockCenter.transform.position.z + _currentBlock.transform.localScale.z / 2 + distance / 2);

                            var slicedBlocks = CutBlock(_currentBlock, newScale, newBlockCenter, cutBlockScale, cutBlockCenter);
                            newCurrentBlock = slicedBlocks[0];
                        }

                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                // Case when block completely misaligns and the game ends.
                OnGameOver(this, EventArgs.Empty);
                return;
            }
        }

        Score++;

        // Increase next block's height
        blockCenter.transform.position = new Vector3(newCurrentBlock.transform.position.x, blockCenter.transform.position.y + _blockHeight, newCurrentBlock.transform.position.z);

        // Create new block
        _currentBlock = Instantiate(blockPrefab);
        _currentBlock.transform.position = blockCenter.position;
        _currentBlock.transform.rotation = Quaternion.identity;
        _currentBlock.transform.localScale = newCurrentBlock.transform.localScale;

        // switch Euler axis every block
        _currentEulerAxis ^= (EulerAxis)1;
        _currentBlock.GetComponent<BlockMovement>().axis = _currentEulerAxis;
    }
}