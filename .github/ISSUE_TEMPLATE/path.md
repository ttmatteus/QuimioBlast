---
name: 6. Nova Funcionalidade - Menor Caminho
about: Sugira uma melhoria ou nova feature para o projeto
title: "[FEATURE] Movimentação do Player via Pathfinding (Click-to-Move)"
labels: enhancement
assignees: ''
---

### 📋 Resumo da Ideia
<!-- Descreva em uma frase o que você gostaria de ver no projeto -->
Implementar um sistema de navegação por clique (*click-to-move*) onde o player calcula e percorre o caminho mínimo até o ponto clicado, desviando automaticamente de obstáculos.

### ⚠️ Problema que Resolve
<!-- Qual dor ou limitação essa feature endereça? -->
Evita que o jogador precise contornar manualmente labirintos ou colisões complexas cenário adentro, além de fornecer uma mecânica de controle mais fluida e estratégica para o estilo do mapa.

### 💡 Solução Proposta
<!-- Descreva como você imagina que essa funcionalidade deveria funcionar -->
* Ao clicar com o mouse (ou tocar na tela) em uma área transitável do mapa, um alvo/marcador visual deve aparecer brevemente na posição clicada.
* O algoritmo de pathfinding (como A* ou o sistema de NavMesh nativo da engine) deve calcular imediatamente a rota ideal.
* O player deve se deslocar suavemente ao longo dos pontos desse caminho, parando ao atingir o destino ou se encontrar um novo obstáculo intransponível no trajeto.

### 🔄 Alternativas Consideradas
<!-- Existe outra forma de resolver o problema? Por que você prefere a solução proposta? -->
A alternativa seria a movimentação direta por eixos (teclado/direcional), mas a solução de pathfinding por clique é essencial para a proposta de exploração e jogabilidade tática planejada para o design deste mapa específico.

### ✅ Critérios de Aceitação
<!-- Como saberemos que essa feature foi implementada corretamente? Liste os critérios -->
- [ ] O player se move até a posição exata do clique se o local for acessível.
- [ ] O player contorna paredes, pilares e zonas de colisão bloqueadas sem ficar travado nas quinas.
- [ ] Se o jogador clicar em um novo ponto enquanto ainda estiver se movendo, o caminho atual é cancelado e o novo caminho mínimo é recalculado instantaneamente.

### 🎨 Mockups / Referências
<!-- Se aplicável, adicione esboços, prints de referência ou links de inspiração -->
* *Referência de mecânica: Estilo clássico de movimentação de jogos como Diablo, League of Legends ou RPGs táticos point-and-click.*

### ℹ️ Contexto Adicional
<!-- Alguma outra informação que ajude a entender a solicitação? -->
É necessário garantir que a malha de navegação (Grid ou NavMesh) seja atualizada corretamente caso existam obstáculos dinâmicos que possam surgir ou se mover pelo cenário.
