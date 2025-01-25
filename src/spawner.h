// Made by George Padron
// Email: george.n.padron@vanderbilt.edu
// Vunetid: padrongn
//
// Spawner class used to spawn nodes.
// Scenes defined by `target` are spawned in intervals defined
// by `time_between_spawns`. Nodes will spawn `spawn_radius` world units
// away from the "player", as defined by `player_path`. Nodes will
// never be spawned outside of the camera. The amount of nodes spawned starts
// at `spawn_count` and increases by one for each wave.
//
// Can be used generically to spawn any kind of Scene, including collectible.
// In this game, this is only used to spawn enemies, however it can be adapted
// to spawn any kind of object.
#ifndef SPAWNER_H
#define SPAWNER_H

#include "godot_cpp/core/defs.hpp"
#include "godot_cpp/variant/node_path.hpp"
#include "godot_cpp/variant/vector2.hpp"
#include <godot_cpp/classes/node2d.hpp>
#include <godot_cpp/classes/packed_scene.hpp>
#include <godot_cpp/core/class_db.hpp>

using namespace godot;

class Spawner : public Node2D {
  GDCLASS(Spawner, Node2D)

private:
  // Inspector exposed fields
  Ref<PackedScene> target;
  double time_between_spawns = 5.0;
  int spawn_count = 1;
  NodePath player_path;
  float spawn_radius = 200;

  // Private member fields
  Node2D *player = nullptr;
  double timer = 0;

protected:
  static void _bind_methods();

public:
  Spawner() = default;
  ~Spawner() = default;

  // Getters and Setters
  double get_time_between_spawns() const noexcept;
  void set_time_between_spawns(const double p_time_between_spawns) noexcept;

  int get_spawn_count() const noexcept;
  void set_spawn_count(const int p_spawn_count) noexcept;

  Ref<PackedScene> get_target() const noexcept;
  void set_target(const Ref<PackedScene> p_target) noexcept;

  NodePath get_player_path() const noexcept;
  void set_player_path(const NodePath p_target) noexcept;

  float get_spawn_radius() const noexcept;
  void set_spawn_radius(const float p_target) noexcept;

  // Override Methods
  void _process(double delta) override;
  void _ready() override;

  // Private methods
  void spawn_wave();
  Vector2 get_spawn_pos() noexcept;
  _FORCE_INLINE_ static float get_random_pos(const float a,
                                             const float b) noexcept;
};

#endif // SPAWNER_H
