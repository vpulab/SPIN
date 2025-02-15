[![arXiv](https://img.shields.io/badge/arXiv-2406.07500-b31b1b.svg)](https://arxiv.org/abs/2406.07500)

# SPIN: An Open Simulator of Realistic Spacecraft Navigation Imagery
![comparison](figs/comparison.jpg)
### Abstract
Data acquired in space operational conditions is scarce due to the costs and complexity of space operations. This poses a challenge to learning-based visual-based navigation algorithms employed in autonomous spacecraft navigation. Existing datasets, which largely depend on computer-simulated data, have partially filled this gap. However, the image generation tools they use are proprietary, which limits the evaluation of methods to unseen scenarios. Furthermore, these datasets provide limited ground-truth data, primarily focusing on the spacecraft's translation and rotation relative to the camera. To address these limitations, we present SPIN (SPacecraft Imagery for Navigation), an open-source realistic spacecraft image generation tool for relative navigation between two spacecrafts. SPIN provides a wide variety of ground-truth data and allows researchers to employ custom 3D models of satellites, define specific camera-relative poses, and adjust various settings such as camera parameters and environmental illumination conditions. For the task of spacecraft pose estimation, we compare the results of training with a SPIN-generated dataset against existing synthetic datasets. We show a 50% average error reduction in common testbed data (that simulates realistic space conditions).

You can read our paper here: [https://arxiv.org/abs/2406.07500](https://arxiv.org/abs/2406.07500)

## Code
We have uploaded the code for the main functionalities of the simulation tool. Note: some textures are not shared, as they belong to a paid package. They are still available in the compiled version. You can use your own textures for them to work.

https://assetstore.unity.com/packages/2d/textures-materials/metals/real-materials-vol-6-aluminum-39949


## Demo
We have a functional pre-release demo here: [pre-release](https://github.com/vpulab/SPIN/releases/tag/pre-release)


## Cite

If you find this useful in your research, please consider citing:

    @article{montalvo2024spin,
        title={SPIN: Spacecraft Imagery for Navigation},
        author={Montalvo, Javier and P{\'e}rez-Villar, Juan Ignacio Bravo and Garc{\'\i}a-Mart{\'\i}n, {\'A}lvaro and Carballeira, Pablo and Besc{\'o}s, Jes{\'u}s},
        journal={arXiv preprint arXiv:2406.07500},
        year={2024}
    }