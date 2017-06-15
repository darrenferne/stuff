"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var hero_service_1 = require("./hero.service");
var HeroesComponent = (function () {
    function HeroesComponent(heroService, router) {
        this.name = 'Tour of Heros';
        this.heroService = heroService;
        this.router = router;
    }
    HeroesComponent.prototype.ngOnInit = function () {
        this.getHeroes();
    };
    HeroesComponent.prototype.getHeroes = function () {
        var _this = this;
        if (this.heroService) {
            this.heroService.getHeroes().then(function (heroes) { return _this.heroes = heroes; });
        }
    };
    HeroesComponent.prototype.addHero = function (name) {
        var _this = this;
        name = name.trim();
        if (!name) {
            return;
        }
        if (this.heroService) {
            this.heroService.addHero(name)
                .then(function (hero) {
                _this.heroes.push(hero);
                _this.selectedHero = null;
            });
        }
    };
    HeroesComponent.prototype.deleteHero = function (hero) {
        var _this = this;
        if (this.heroService && hero) {
            this.heroService.deleteHero(hero.id)
                .then(function () {
                _this.heroes = _this.heroes.filter(function (h) { return h !== hero; });
                if (_this.selectedHero === hero) {
                    _this.selectedHero = null;
                }
            });
        }
    };
    HeroesComponent.prototype.onSelect = function (hero) {
        this.selectedHero = hero;
    };
    HeroesComponent.prototype.gotoDetail = function () {
        this.router.navigate(['/hero', this.selectedHero.id]);
    };
    return HeroesComponent;
}());
HeroesComponent = __decorate([
    core_1.Component({
        selector: 'my-heroes',
        templateUrl: './heroes.component.html',
        styleUrls: ['./heroes.component.css']
    }),
    __metadata("design:paramtypes", [hero_service_1.HeroService, router_1.Router])
], HeroesComponent);
exports.HeroesComponent = HeroesComponent;
//# sourceMappingURL=heroes.component.js.map