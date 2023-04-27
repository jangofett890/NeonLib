The beginning.

NeonLib: A Comprehensive Unity Project Framework
Overview:
NeonLib is a comprehensive Unity project framework designed to provide a flexible and extendable environment for game development. It features a collection of versatile packages that cater to various aspects of game mechanics, including GameEvents, MicroGames, StateMachine, IK-PoseRig, WorldStreaming, NPC, Inventories, Templates, a Modding API, ECS/DOTS Integration, Resource/File saving utilities, a Localization system, and a Debug system. Each package is crafted to meet specific needs within a game development project, making it easier for developers to create and maintain complex systems.
The project's primary goal is to combine the flexibility of the Unity Editor and the modability of the Creation Engine, providing a fertile ground for creating RPGs and making game development an enjoyable experience.
GameEvents:
Description:
A robust event system based on ScriptableObjects, allowing for efficient communication between game components without tight coupling. It supports easy integration with Unity's DOTS/ECS for high-performance applications. It also will have a visualization system so developers can easily see where events are being called from and where the events fire to.
Known Types:
GameEvent: A ScriptableObject representing a game event that can be raised and listened to by other objects.
GameEventListener: A ScriptableObject that listens for GameEvents and responds to them by invoking UnityEvents or UOMethodInvokers.
UOMethodInvoker: A ScriptableObject that stores a target object and a method to invoke on that object when triggered.
CustomEventPlayable: Integrates GameEvents and other NeonLib packages into the unity Timeline and Playables package, allowing a scripted timeline that can control microgames, and other event based game systems.
CustomEventPlayableBehaviour: The behavior class associated with CustomEventPlayable.
CustomEventPlayableEditor: Inspector for the CustomEventPlayable
GameEventListenerEditor: Inspector for the GameEventListener
UOMethodInvokerEditor: Inspector for UOMethodInvoker
Suggested/Implied Types:
GameEventChannel: A centralized event hub to manage multiple GameEvents and their listeners.
ConditionalGameEventListener: A GameEventListener that only responds to events if a specified condition is met.
MicroGames:
Description:
A package that provides a system to use GameEvents, StateMachines, and other NeonLib systems to create easy to integrate minigames into your game. It includes a collection of small, reusable game mechanics and components that can be easily combined and customized to create unique gameplay experiences.
Known Types:
MicroGame: A scriptable object that contains the information for a MicroGame, as well as a state machine that's instantiated at runtime that drives the microgame logic. There are also GameEvents invoked for when the MicroGame starts and ends as well as before to allow for other systems to integrate and prepare for MicroGame behavior.
MicroGameEventHandler: A monobehavior that has fields for GameEventListeners for the StartGame, BeforeStart, and OnEnd events associated with the microgame, this class is currently placeholder and may not be needed.
MicroGameInputHelper: A helper function static class that contains common functions for doing simple MiniGame things, some functions include EvaluatingTimingEvent, CalculatingScoreMultiplier, etc. This class needs heavy work and will be extended through development.
MicroGameInputResponse: A scriptableobject that is used to invoke events when the player presses a specific button when a microgame is active. It has different response types that do different things to integrate with the NeonLib system, such as Invoking a GameEvent, Inlining a UOMethodInvoker editor to invoke a specific function, a UnityEvent option, Change a State Machines state to a target state, MicroGameManager option that can start or stop a MicroGame or interface with any other options on a MicroGame.
MicroGameManager: A global MicroGameManager monobehavior that is used to start and stop a microgame by name and ticks the MicroGame’s statemachine.
MicroGameEditor: The MicroGame inspector.
MicroGameInputResponseEditor: The MicroGameInputResponse inspector.
Suggested/Implied Types:
Timer: A countdown timer that triggers events when the timer reaches zero.
Collectible: An item that can be collected by the player and added to their inventory or used to trigger events.
Spawner: A system for spawning game objects such as enemies, collectibles, or obstacles.
PlatformerController: A character controller specifically designed for 2D platformer games.
StateMachine:
Description:
A powerful state machine system for managing game object behaviors, AI, animations, and other state-driven elements.
Known Types:
StateMachine: A scriptable object with a currentState and previousState, with a function to Initilize it, to ChangeState, to tick the current state, and to reset the state machine. Needs changes to accommodate BehaviorStates and the StateMachine editor itself.
State: A scriptable object with fields for 3 game events for on enter, exit, and update, and virtual functions to invoke them, as this is a base class to be extended by more advanced states.
Suggested/Implied Types:
BehaviorState: extends state, a base state that implements behaviors and behavior rules by Mapping Behaviors to a List of Behavior Rules that must be met before the List’s corresponding Behavior is added to the Active Behavior list. 
Behavior: A SO that defines a base behavior that has fields for GameEvents onActivate onDeactivate, and virtual methods called Activate and Deactivate that also invokes the GameEvents if not null. Also has a weight to define importance over other behaviors.
BehaviorRule: A SO that defines what is required to transition from behavior to behavior the base rule uses ScriptableVariables and basic comparators for the CheckRule method.
StateMachineEditor: An inspector for StateMachine that is quite advanced, it inlines a StateMachineGraphView as well as a list of available States when no node is selected, as well as a tree of current states to allow easy selection with the interactive graph view. When a BehaviorState is entered the StateMachineGraphView is switched to the BehaviorStateGraphView which allows for the editing of the Behaviors in a BehaviorState within a StateMachineEditor.
StateMachineGraphView: A graphview that allows different states to be managed and changed via StateMachineEditor.
BehaviorStateGraphView: A graphview that allows the behaviors to be linked to behavior rules in a BehaviorState, this view is also used for any states that inherit BehaviorState as their base class.
IK-PoseRig:
Description:
An easy-to-use generative character animation system that can handle bipedal, quadrupeds, and anything in-between using Unity's IK system. The design goal is to import any character animation from any rig and map it to any other character, while allowing for customizable behaviors and seamless integration with existing Unity systems, as well as the NeonLib system. It also stores data as ScriptableObjects to allow for easy use with ECS and DOTS and the modding API. Bidirectional motion transfer is also another planned feature.
Known Types:
(None created yet)
Suggested/Implied Types:
IKSolver: A base class for inverse kinematics solvers.
IKRig: A class that contains the required bones, solvers and targets required to build an IK rig to animate.
IKBone: A definition of a bone.
IKChain: A definition for limbs and other IK chains that are used by solvers.
IKTarget: A definition for a target for certain IK Solvers.
RigImporter: Imports and creates rigs directly from models, the unity Avatar system, and the Legacy animation system.
RigEditor: Editor for IKRig that displays the bones, chains, targets, and solvers, and allows for easy creation of new IKRigs.
RigValidator: A static class used to determine if a IKRig can be properly applied to a model then provides helper functions to allow the IKAnimator to animate a SkinnedMeshRenderer given the bones it has from the model and applies any mathematical helper functions to make this seamless for the animator.
IKAnimator: A MonoBehavior that is used to animate the SkinnedMeshRenderer with an IKRig and it’s solvers, and base IKAnimations and seamlessly blends between them as needed. This is controlled by a StateMachine that uses BehaviorRules to 
IKAnimation: Defines the animation of `Limbs` (IKChains) and other IK properties over time, this allows for a base animation to be imported from a model, and then the IKAnimator can use it and blend between it and physics solvers
IKAnimationImporter: Creates and saves IKAnimation SOs from a existing animation stored on a model which finds it’s IKRig or generates one if one doesnt exist already.
CharacterInteractionManager: Helper class to allow for dynamic IKRig to IKRig interactions.
IKRigEditor
IkAnimationManager
IKRigManager
IKAnimationEditor
WorldStreaming:
Description:
A dynamic world streaming solution that allows for efficient loading and unloading of game world assets, ensuring smooth and seamless gameplay experiences.
Known Types:
(None created yet)
Suggested/Implied Types:
WorldSystem: Manages the game world including streaming, props, terrain, and object persistence. Initializes subsystems such as WorldStreamingManager, PropSystem, ObjectPersistenceManager, and TerrainStreamingSystem, and provides easy access to this data for other systems.
WorldData: A scriptable object that contains data for a Game World, including all the scenes associated with it and any other respective SO data. Manages serialization of world data and scene data instances, and allows for managing this via editors as needed.
SceneData: A SO that contains data for individual scenes including prop placements, NPC starting position and encounter data, as well as terrain chunks. Manages the serialization and deserialization of the scene data to allow for scenes to be constructed from this data at runtime, and editing through the scene view and editor windows.
PropData: Contains Data for individual props in the scene, such as inventory items, physics items, puzzle items, etc. Also exposes this data to other systems and a custom inspector for this data.
NPCData: Contains data to construct NPCs with integration with the NPC system. This data is specifically used for the world streaming system to initialize the NPC’s behavior and scheduling and needs system into the ECS game world, allowing the data to be streamed and the NPC game object to be constructed when needed. Editor for this and the NPC system should be heavily integrated.
TerrainChunkData: Contains data for terrain chunks in the scene and manages serialization and deserialization of such data.
WorldStreamingManager: Manages the streaming of the current Game World given the players position, integrates CellManager and StreamingAreas to access and process the data. 
CellManager: Manages the cells within the streaming system and the systems within the cells. Has access to cell data such as the boundaries and streaming radius, it determines the cell states and manages the loading and unloading of the cell data.
StreamingArea: Defines an additional area within the current Game World for additional Game Worlds to be streamed in as needed, this allows for dynamic loading of interiors, with the main Game World being loaded and cached, as normal, and the interiors loaded and supplanted on top of the current game world. This also allows for modders to mod interiors and exteriors without much conflict.This would allow the editor to display only one Game World at a time to allow a better focus on what is being developed where.
PropSystem: Manages the placement, state, and streaming in of props in the game world, integrates with the World Streaming Manager, and instantiates props as ECS entities as needed. This allows props to exist in the data world for NPCs to find during schedules without physics and additional data being needed to simulate this.
Prop: An ecs entity that represents a prop and integrates with prop system and Prop Component.
PropComponent: Contains the data for individual props.
ObjectPersistenceManager: Manages prop persistence in the game world creating RuntimeObjectData records in integration with the WorldStreamingManager to ensure prop movements and alterations through game saves are persistent.
RuntimeObjectData: Contains data about runtime object states, such as position, rotation, and scale, and is integrated by the game save load system with the ObjectPersistenceManager to allow persistent prop location throughout game saves.
TerrainStreamingSystem: Manage the streaming of the terrain chunks into the game world, note only the primary Game World can use the TerrainStreamingSystem. Integrates with TerrainChunk, TerrainLodManager, and the WorldStreamingManager to instantiate and manage terrain chunks while handling loading and unloading based on streaming.
TerrainChunk: Represents a terrain chunk in the game world, defining its size and possibly allowing for terrain modification. Integrates with TerrainChunkData and TerrainStreamingSystem to instantiate TerrainChunkData as terrain in the game world via the TerrainStreamingSystem.
TerrainLODManager: Manages LOD for terrain chunks by integrating with TerrainChunk, TerrainChunkData, and TerrainStreamingSystem, updating TerrainChunk meshes as required for the LOD determined by the player positon.
LODSystem: Manages LOD for non terrain chunks, ensuring best use LOD practices for larger assets such as nature and buildings, which have their own MeshLOD integrations with Unity.
Various Editor Extensions and Editors: Many of these classes need custom editors to allow for seamless integration into unity, as well as an easy to use environment for new developers familiar with unity or the Creation Engine. As well as provide a way for the Scene window to be used to develop Worlds by defining a size and streaming empty chunks with a default terrain system and real time capture and structuring of placing gameobjects in the world based on their position.
NPC:
Description:
A comprehensive non-player character system featuring AI, behavior trees that integrate with StateMachine, and dialogue management to create engaging and interactive NPCs.
Known Types:
(None created yet)
Suggested/Implied Types:
NPCController: A MonoBehaviour for managing NPC behavior using StateMachine, including movement, interaction, and dialogue.
Dialogue: A ScriptableObject representing a conversation between the player and an NPC.
NPCSystem: Allows the changing of global NPC behaviors, schedules, and needs in the game world, integrates with NPCManager, NPCSchedule, NPCNeeds, and WorldStreamingManager. Instantiates and manages NPCs as ECS entities, loading and unloading their game representations as needed, and simulates the NPC behavior when unloaded.
NPCManager: Manages Indvidual NPCs within the NPCSystem, accesses NPCData, NPCSchedule, and NPCNeeds to instantiate and manage NPC entities, scheduling, and needs.
NPCSchedule: A scriptable object that contains the schedule for individual NPCs, or a generic schedule to be used a route. Serializes and Deserializes required data and integrates with the NPCManager, and allows for the schedule to be edited through a custom inspector.
NPCNeeds: Contains data for individual NPC needs and consumption. Serializes and Deserializes the needed data and allows for integration with the NPCManager and custom inspector.
NPC (ECS Entity): Represents an NPC in the game world as a entity, stores the NPC data and state and interacts with other systems as needed.
NPCComponent: ECS data component for the NPC entity such as the NPCs schedule, needs, and behaviors.Interacts with the NPCSystem and other ECS systems.
NPCJob (ECS JOB): Executes required job logic for a task or behavior system to enable compatibility between the NPCSystem and ECS architecture and StateMachines, and updates NPCComponent data as needed.
NPCSystem (ECS System): Manages the overall behavior and state of the NPCs in the ECS game world, integrates with NPCComponent, NPCJob, the regular NPCSystem and WorldStreamingManager. Separates loaded NPCs from unloaded NPCs by simulating unloaded NPC behavior in ECS. 
PathfindingSystem: Interacts with NPCSystem to provide a path that an NPC is on, and what position the NPC would be at given a time to evaluate by the streaming manager. Allows the NPC to dynamically change its pathing in the ECS world and still behave properly in the game and ecs world. Integrates with NavMeshIntegration to get appropriate positions depending on the pathing restrictions and stability.
NavMeshIntegration: Integrates with Unity’s built in NavMesh system to provide NavMesh navigation for NPC entities by finding a clever solution to allow for unloaded cells to provide NavMesh data to the PathfindingSystem in the ECS world.
Inventories:
Description:
A versatile inventory management system that allows for the creation, storage, and manipulation of in-game items, equipment, and other assets.
Known Types:
(None created yet)
Suggested/Implied Types:
Inventory: A ScriptableObject representing a container for storing items and managing their quantities.
Item: A ScriptableObject representing a base class for various in-game items, such as weapons, armor, consumables, and quest items.
Equipment: A subclass of Item that represents wearable gear, such as armor, weapons, and accessories.
Todo
Templates:
Description:
A collection of pre-built templates and systems for rapid game development, including character controllers, NeonLib package integration such as StateMachine and Inventories, physics systems, and UI elements.
Known Types:
HumanoidPhysicsBasedController: A physics based controller meant to control humanoid bipedal entities. Has a ton of controls and settings but it’s primary means of function is via floating on a raycast, thus allowing for advanced game world physics interaction and movement. It uses animation curves and other physics systems to allow for realistic movement in any terrain or gravity, even on planets.
PlayerInputForHumanoidEntity: Uses the PlayerInput class and the HumanoidPhysicsBasedController to drive player input to a player character using the aforementioned character controller.
Suggested/Implied Types:
FirstPersonController: A template for creating first-person character controllers.
ThirdPersonController: A template for creating third-person character controllers.
UIMenu: A pre-built UI menu system for navigating between game scenes and managing game settings.
TemplateTypes for States, StateMachines, Inventories, MicroGames, etc.
Modding API:
Description:
An API that enables end-users to create, share, and integrate mods and custom content into the game at runtime, fostering a vibrant modding community and extending the game's lifespan.
Known Types:
(None created yet)
Suggested/Implied Types:
ModLoader: A system for loading and managing mods from external sources.
ModAPI: A set of tools and interfaces for modders to interact with and extend the game's systems.
Variables:
Description:
An essential part of NeonLib is the ScriptableVariable, and other data types like it, which allow storing data safely as a Unity Engine Object  reference or a JSON string.
Known Types:
ScriptableVariable: Has a Value and A ValueType that is used to determine the value in the editor, and easily set the value in the editor and via code. Uses NeonLibSerilizationSettings for JSON serialization and deserialization. 
RuntimeSet: Similar to scriptable variables and may undergo changes for proper serilization as current it uses a T symbol in the class definition for a list of items of T, and provides GameEvent fields for when the list is modified, or specifically if an item is removed or added. It invokes these events with parameters such as the count for onlistchanged, and the item added or removed for the respective events.
ShowInScriptableVariableAttribute: An Attribute that allows your type to be shown as a type in the list for ScriptableVariable
MenuField: a custom field type that allows ScriptableVariable to use a GenericMenu to properly display Types in its inspector.
ScriptableVariableEditor: The inspector for ScriptableVariable which makes use of MenuField and provides a TypeMapping class which uses the Attribute and makes basic types selectable in the MenuField and displays their proper editor fields.
Suggested/Implied Types:
Todo?

ECS/Dots
Description:
A package focused on integrating Unity's Data-Oriented Technology Stack (DOTS) and Entity Component System (ECS) with the existing ScriptableObject data systems of the NeonLib project. This package aims to provide performance benefits at runtime, ensuring efficient and scalable solutions for complex game mechanics.
Known Types:
(None created yet)
Suggested/Implied Types:
EntityConversionSystem: A system for converting ScriptableObject-based data to DOTS-compatible entities and components.
DOTSUpdateSystem: A system for updating and managing DOTS entities during runtime, following the logic defined in the ScriptableObject-based systems.
DOTSDataBridge: A set of utility classes and functions to facilitate the communication between the traditional Unity GameObject/Component systems and the DOTS/ECS systems.
Debug:
Description:
A small package that contains LogChannel and NeonDebug which allows for using custom debug logging with different colors and channels indicated in the console.
Known Types:
LogChannel: A ScriptableObject that is saved to the resources folder so it can be dynamically loaded by NeonDebug, which has a Name, Description, PrintColor, and Enable boolean. There is also a UnityEvent which is invoked with the string via the OnLog function.
NeonDebug: A static class that caches the LogChannels and allows them to be used via a string search for them via their name, and also invokes the OnLog function when the Log function is used.

Suggested/Implied Types:
GameLogChannel: Debugging tool for runtime use that allows for GameEvents to be invoked with parameters via code as well as print a debug message. This could be useful for building a developer mode.
Localization
Description:
A small package that allows for ScriptableObjects to be used to create a dynamic string library with different strings for different languages with audio support.
Known Types:
LocaleAsset: a SO that has 2 <string, string> dictionaries, one with localized strings, the other with localized audio clip paths.
LocalizationManager: A static class that manages Localization interface with scriptable objects and json. This allows for shipping a full JSON file that can be easily editable by users at runtime, and for dynamic locale generation using these Json files.
LocalizedAudioClip: A SO that has an ID, and default value as well as a function to get the localized value.
LocalizedString: A SO that has an ID, and a default value, as well as a function to get the localized value.

Suggested/Implied Types:
Not Sure??
ResourceManagement
Description:
A small package that contains helper classes and functions to serialize data to disk.
Known Types:
NeonResources: the main helper class that has the majority of functions to save binary data to disk, such as SaveObjectToFilePath, SaveTextToFilePath, LoadObjectFromFilePath, LoadTextFromFile, SaveGameSaveToSaveDataPath, LoadGameSaveFromSaveDataPath, Serialize, Deserialize
NeonLibSerializationSettings: A static class that contains settings for NewtonSoft Json serializer.
ColorJsonConverter: A JsonConverter for unity’s Color struct.
Suggested/Implied Types:
Additional JSON Converters as needed.

Summary:
NeonLib is a comprehensive Unity project framework that combines the flexibility of the Unity Editor with the modability of the Creation Engine. By offering a wide array of packages, each tailored to specific aspects of game development, NeonLib provides an environment where developers can create engaging and immersive experiences. With a focus on modability, performance, and ease of use, NeonLib aims to make game development not only more efficient but also more enjoyable, particularly for RPG development.
