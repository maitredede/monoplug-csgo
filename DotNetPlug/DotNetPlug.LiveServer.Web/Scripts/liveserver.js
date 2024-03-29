﻿"use strict";

angular.module("liveserver", ["SignalR", "ui.router", "ui.bootstrap"])
.controller("demoController", ["$scope", "Hub", "$timeout", "GameEvent", "$injector",
    function ($scope, Hub, $timeout, GameEvent, $injector) {
        $scope.error = "";
        $scope.lastEvents = [];
        $scope.players = [];
        $scope.bomb = {
            planting: false,
            planted: false,
            ticking: false,
            exploded: false,
            defused: false,
        };

        //Default config
        $scope.serverId = "Demo";
        //Real config
        if ($injector.has("lsConfig")) {
            $injector.invoke(["lsConfig", function (lsConfig) {
                $scope.serverId = lsConfig.serverId;
            }]);
        }

        var hub = null;
        $scope.hubConnectDone = function hubConnectDone() {
            $scope.error = "connected : (status=" + hub.connection.state + ")";
            hub.hello($scope.serverId).done(function () {
                $scope.$apply(function () {
                    $scope.error = "listening...";
                })
            });
        };

        $scope.hubConnectFail = function hubConnectFail(reason) {
            console.log(reason);
            $scope.error = reason;
        };

        var connect = function () {
            hub.connection.start()
                .done(function (e) {
                    $scope.$apply(function () {
                        $scope.hubConnectDone();
                    });
                })
                .fail(function (reason) {
                    $scope.$apply(function () {
                        $scope.hubConnectFail(reason);
                    });
                });
        };

        hub = new Hub("webUIHub", {
            listeners: {
                RaiseEvent: function (serverId, e) {
                    $scope.$apply(function () {
                        $scope.hubRaiseEvent(serverId, e);
                    });
                },
                SetPlayers: function (serverId, players) {
                    $scope.$apply(function () {
                        $scope.hubSetPlayers(serverId, players);
                    });
                }
            },
            methods: ["hello"],
            errorHandler: function (error) {
                console.error(error);
                $scope.error = error;
            },
            hubDisconnected: function () {
                $scope.$apply(function () {
                    $scope.error = "connecting...";
                });
                connect();
            }
        });

        $scope.hubSetPlayers = function hubSetPlayers(serverId, players) {
            $scope.players = players;
        }
        //var stepExec = function stepExec(thisTick, nextTick) {
        //    if (thisTick) {
        //        thisTick();
        //    }
        //    if (nextTick) {
        //        $timeout(function () { nextTick() }, 0);
        //    }
        //}
        var playerStepExec = function playerStepExec(userid, thisTick, nextTick) {
            for (var i = 0; i < $scope.players.length; i++) {
                var p = $scope.players[i];
                if (p.Id == userid) {

                    if (thisTick) {
                        thisTick(p);
                    }
                    if (nextTick) {
                        $timeout(function () { nextTick(p) }, 0);
                    }

                    break;
                }
            }
        }

        $scope.hubRaiseEvent = function hubRaiseEvent(serverId, e) {
            switch (e.name) {
                case GameEvent.weapon_fire:
                    playerStepExec(e.userid, function (p) { p.weapon_fire = true; }, function (p) { p.weapon_fire = false; })
                    return;
                case GameEvent.enter_buyzone:
                    playerStepExec(e.userid, function (p) {
                        p.buyzone_in = true;
                        p.buyzone_canbuy = e.canbuy;
                    });
                    return;
                case GameEvent.exit_buyzone:
                    playerStepExec(e.userid, function (p) {
                        p.buyzone_in = false;
                        p.buyzone_canbuy = e.canbuy;
                    });
                    return;
                case GameEvent.enter_bombzone:
                    playerStepExec(e.userid, function (p) {
                        p.bombzone_in = true;
                        p.bombzone_hasbomb = e.hasbomb;
                    });
                    return;
                case GameEvent.exit_bombzone:
                    playerStepExec(e.userid, function (p) {
                        p.bombzone_in = false;
                        p.bombzone_hasbomb = e.hasbomb;
                    });
                    return;


                case GameEvent.bomb_beginplant:
                    $scope.bomb.planting = true;
                    break;
                case GameEvent.bomb_abortplant:
                    $scope.bomb.planting = false;
                    break;
                case GameEvent.bomb_planted:
                    $scope.bomb.planting = false;
                    $scope.bomb.planted = true;
                    break;
                case GameEvent.bomb_defused:
                    $scope.bomb.planting = false;
                    $scope.bomb.planted = false;
                    break;
                case GameEvent.bomb_exploded:
                    $scope.bomb.planting = false;
                    $scope.bomb.planted = true;
                    break;
                case GameEvent.bomb_beep:
                    $scope.bomb.ticking = true;
                    $timeout(function () { $scope.bomb.ticking = false; }, 0);
                    break;
                    //case GameEvent.entity_killed:
                    //case GameEvent.player_death:
                    //case GameEvent.bomb_begindefuse:
                    //case GameEvent.bomb_abortdefuse:

                case GameEvent.bomb_dropped:
                    for (var i = 0; i < $scope.players.length; i++) {
                        var p = $scope.players[i].hasbomb = false;
                    }
                    break;
                case GameEvent.bomb_pickup:
                    playerStepExec(e.userid, function (p) {
                        p.hasbomb = true;
                    });
                    break;

                case GameEvent.player_hurt:
                    playerStepExec(e.userid, function (p) {
                        p.Health = e.health;
                        p.Armor = e.armor;
                    })
                    return;
                case GameEvent.player_blind:
                    playerStepExec(e.userid, function (p) { p.blinded = true; }, function (p) { p.blinded = false; })
                    break;

                case GameEvent.player_jump:
                case GameEvent.weapon_reload:
                case GameEvent.item_pickup:
                case GameEvent.player_avenged_teammate:
                case GameEvent.item_equip:
                    //Ignoring
                    return;
            }
            $scope.lastEvents.splice(0, 0, e);
            while ($scope.lastEvents.length > 50) {
                $scope.lastEvents.splice(49, 1);
            }
        };

        connect();
    }])