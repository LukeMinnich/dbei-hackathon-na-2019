### Problem Statement
* What problem are we trying to solve
    * Given site constraints, eliminate poor building configurations based on excesssive structural cost
* What are we not trying to solve
    * Architectural programming
    * Formfinding in the architectural sense
* What skillsets do we have at our disposal
    * Clifton - presentation, expertise in drawing buildings with math (at the macro level), other engineers who can do front end development if needed
    * Anthonie - geometric visualization in Rhino & Grashopper, parametric models for optimization, structural psuedo-design & analysis
    * Luke - .Net as it relates to geometry, generate variations, set up any databases, front-end angular app dev
    * Mathias - Machine Learning, database queries, Python, AWS, Jupyter notebook
* What is our technology stack
    * Rhino/ Grasshopper
    * SQL Lite
    * Jupyter
    * Python

### Before we get started
* Determine randomizer parameters and ranges
    * Unit mix (3 variations)
        * Tolerance
    * Unit layout (4 variations)
    * Typologies (6 variations)
    * Dimensional constraints
        * Vary depending on the typology
        * Limit to multiple of 6 feet
    * Seismicity (Sds - 0.5, 1.0, 1.5)
    * number of stories (2 - 5)
    * Drift limit (0.15, 0.20, 0.25)
* Determine performance metrics (both binary and quantitative)
    * Total structural shear wall cost
    * Do all walls meet upper limit on force
    * Does drift meet upper limit
* Determine database table schema

### Structural considerations
* Consider the impact of rho
* Consider a nonlinear wall-cost model

### Task Breakdown

* Luke
    * Set up git repo
    * Rigid analysis & wall evaluation widget
    * Database setup
        * What database format?
    * Variation generator
* Anthonie
    * Unit layout generation (from CAD layer - could be a rectangle or something more complex) to get close to target unit mix
    * 
* Mathias
    * Set up Jupyter notebook
    * Consider different Machine Learning models
    * Consider AWS for training the model
    * Set up an EC2 instance (use a bucket if it needs to)
* Clifton
    * Visuals, graphics
    * Business case
    * Presentation (Powerpoint)
    * Market data

