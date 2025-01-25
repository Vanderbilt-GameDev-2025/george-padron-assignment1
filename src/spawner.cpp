// Made by George Padron
// Email: george.n.padron@vanderbilt.edu
// Vunetid: padrongn

#include "spawner.h"
#include "godot_cpp/classes/global_constants.hpp"
#include "godot_cpp/classes/node2d.hpp"
#include "godot_cpp/classes/object.hpp"
#include "godot_cpp/classes/packed_scene.hpp"
#include "godot_cpp/classes/ref.hpp"
#include "godot_cpp/core/class_db.hpp"
#include "godot_cpp/core/error_macros.hpp"
#include "godot_cpp/core/object.hpp"
#include "godot_cpp/core/property_info.hpp"
#include "godot_cpp/variant/node_path.hpp"
#include "godot_cpp/variant/variant.hpp"
#include "godot_cpp/variant/vector2.hpp"
#include <cstdlib>
#include <godot_cpp/classes/engine.hpp>
#include <random>

using namespace godot;

// Magic numbers that correspond to the edges of the camera
// Could calculate this dynamically but would take too much
// effort for the scope of this project
const int CAMERA_X_MIN = -576;
const int CAMERA_X_MAX = 576;
const int CAMERA_Y_MIN = -328;
const int CAMERA_Y_MAX = 328;

// Efficient Random generator
static std::mt19937 gen(std::random_device{}());

void Spawner::_bind_methods() {

  // Bind getters and setters
  ClassDB::bind_method(D_METHOD("get_time_between_spawns"),
                       &Spawner::get_time_between_spawns);
  ClassDB::bind_method(
      D_METHOD("set_time_between_spawns", "p_time_between_spawns"),
      &Spawner::set_time_between_spawns);

  ClassDB::bind_method(D_METHOD("get_spawn_count"), &Spawner::get_spawn_count);
  ClassDB::bind_method(D_METHOD("set_spawn_count", "p_spawn_count"),
                       &Spawner::set_spawn_count);

  ClassDB::bind_method(D_METHOD("get_target"), &Spawner::get_target);
  ClassDB::bind_method(D_METHOD("set_target", "p_target"),
                       &Spawner::set_target);

  ClassDB::bind_method(D_METHOD("get_player_path"), &Spawner::get_player_path);
  ClassDB::bind_method(D_METHOD("set_player_path", "p_player_path"),
                       &Spawner::set_player_path);

  ClassDB::bind_method(D_METHOD("get_spawn_radius"),
                       &Spawner::get_spawn_radius);
  ClassDB::bind_method(D_METHOD("set_spawn_radius", "p_spawn_radius"),
                       &Spawner::set_spawn_radius);

  // Bind helper functions
  ClassDB::bind_method(D_METHOD("spawn_wave"), &Spawner::spawn_wave);
  ClassDB::bind_method(D_METHOD("get_spawn_pos"), &Spawner::get_spawn_pos);

  // Expose fields to inspector
  ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "time_between_spawns"),
               "set_time_between_spawns", "get_time_between_spawns");
  ADD_PROPERTY(PropertyInfo(Variant::INT, "spawn_count"), "set_spawn_count",
               "get_spawn_count");
  // Ensure that this is exposed as a Scene
  ADD_PROPERTY(PropertyInfo(Variant::OBJECT, "target",
                            PROPERTY_HINT_RESOURCE_TYPE, "PackedScene"),
               "set_target", "get_target");
  ADD_PROPERTY(PropertyInfo(Variant::NODE_PATH, "player_path"),
               "set_player_path", "get_player_path");
  ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "spawn_radius"), "set_spawn_radius",
               "get_spawn_radius");
}

// Getters and Setters
double Spawner::get_time_between_spawns() const noexcept {
  return time_between_spawns;
}
void Spawner::set_time_between_spawns(
    const double p_time_between_spawns) noexcept {
  time_between_spawns = p_time_between_spawns;
};

Ref<PackedScene> Spawner::get_target() const noexcept { return target; }
void Spawner::set_target(const Ref<PackedScene> p_target) noexcept {
  target = p_target;
};

int Spawner::get_spawn_count() const noexcept { return spawn_count; }
void Spawner::set_spawn_count(const int p_spawn_count) noexcept {
  spawn_count = p_spawn_count;
};

NodePath Spawner::get_player_path() const noexcept { return player_path; }
void Spawner::set_player_path(const NodePath p_player_path) noexcept {
  player_path = p_player_path;
};

float Spawner::get_spawn_radius() const noexcept { return spawn_radius; }
void Spawner::set_spawn_radius(const float p_spawn_radius) noexcept {
  spawn_radius = p_spawn_radius;
};

// Override functions
void Spawner::_ready() {
  // If in editor, don't run any process code
  if (Engine::get_singleton()->is_editor_hint()) {
    set_process(false);
    return;
  }

  // Initialize the player if set
  if (player_path.is_empty()) {
    ERR_PRINT("Player path is empty! Please set a valid player path");
    return;
  }
  player = get_node<Node2D>(player_path);
}

void Spawner::_process(double delta) {
  // Spawn waves on each timer interval
  timer -= delta;
  if (timer <= 0.0) {
    spawn_wave();
    timer += time_between_spawns;
  }
}

// Private functions
void Spawner::spawn_wave() {
  for (int i = 0; i < spawn_count; ++i) {
    // Ensure the target is a valid scene
    if (!target.is_valid()) {
      ERR_PRINT("Target is invalid!");
      return;
    }

    // Instantiate the target
    Node *instanced = target->instantiate();
    Node2D *instance_node2d = Object::cast_to<Node2D>(instanced);
    if (!instance_node2d) {
      ERR_PRINT("Target node failed to spawn properly!");
      return;
    }

    // Now set the position of the newly instantiated node
    instance_node2d->set_position(get_spawn_pos());
    add_child(instance_node2d);
  }

  ++spawn_count;
}

// Generates a random position between `a` and `b`
float Spawner::get_random_pos(const float a, const float b) noexcept {
  std::uniform_real_distribution dist(a, b);
  return dist(gen);
}

// Generates valid spawn positions for generated Nodes to use
Vector2 Spawner::get_spawn_pos() noexcept {
  Vector2 player_pos = player->get_position();

  while (true) {

    // Get random values within the accepted camera raidus
    float x = get_random_pos(CAMERA_X_MIN, CAMERA_X_MAX);
    float y = get_random_pos(CAMERA_Y_MIN, CAMERA_Y_MAX);

    // Only accept values 'spawn_radius' units away from
    // the player's position
    float dist = (Vector2(x, y) - player_pos).length();
    if (dist > spawn_radius) {
      return Vector2(x, y);
    }
  }
}
