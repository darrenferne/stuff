import {
    Component,
    OnInit
} from '@angular/core';
import { Router } from '@angular/router';
import { Hero } from './hero';
import { HeroService } from './hero.service';

@Component({
    selector: 'my-heroes',
    templateUrl: './heroes.component.html',
    styleUrls: ['./heroes.component.css']
})

export class HeroesComponent implements OnInit {
    private heroService: HeroService;
    private router: Router

    name = 'Tour of Heros';
    selectedHero: Hero;
    heroes: Hero[];

    constructor(heroService: HeroService, router: Router) {
        this.heroService = heroService;
        this.router = router;
    }

    ngOnInit(): void {
        this.getHeroes();
    }

    getHeroes(): void {
        if (this.heroService) {
            this.heroService.getHeroes().then(heroes => this.heroes = heroes);
        }
    }

    addHero(name: string) {
        name = name.trim();
        if (!name) { return; }
        if (this.heroService) {
            this.heroService.addHero(name)
                .then(hero => {
                    this.heroes.push(hero);
                    this.selectedHero = null;
                });
        }
    }

    deleteHero(hero: Hero) {
        if (this.heroService && hero) {
            this.heroService.deleteHero(hero.id)
                .then(() => {
                    this.heroes = this.heroes.filter(h => h !== hero);
                    if (this.selectedHero === hero)
                    {
                        this.selectedHero = null;
                    }
                });
        }
    }
    onSelect(hero: Hero): void {
        this.selectedHero = hero;
    }

    gotoDetail(): void {
        this.router.navigate(['/hero', this.selectedHero.id]);
    }
}
