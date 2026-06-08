---
name: 4. Nova Funcionalidade - Ataque
about: Sugira uma melhoria ou nova feature para o projeto
title: "[FEATURE] Sistema de Combate: Ataque do Jogador"
labels: enhancement
assignees: ''
---

### 📋 Resumo da Ideia
<!-- Descreva em uma frase o que você gostaria de ver no projeto -->
Implementar a mecânica básica de ataque para o personagem principal interagir ofensivamente com o ambiente e inimigos.

### ⚠️ Problema que Resolve
<!-- Qual dor ou limitação essa feature endereça? -->
O jogo atualmente é puramente de exploração pacífica, impedindo dinâmicas de combate ou defesa contra ameaças.

### 💡 Solução Proposta
<!-- Descreva como você imagina que essa funcionalidade deveria funcionar -->
Ao clicar com o botão esquerdo do mouse (ou botão de ação), o personagem deve executar uma animação de ataque, gerando uma área de colisão (hitbox) temporária que causa dano a entidades inimigas nessa área.

### 🔄 Alternativas Consideradas
<!-- Existe outra forma de resolver o problema? Por que você prefere a solução proposta? -->
*   **Ataque automático por proximidade:** Reduz o dinamismo. O ataque manual focado em timing gera mais engajamento e habilidade por parte do jogador.

### ✅ Critérios de Aceitação
<!-- Como saberemos que essa feature foi implementada corretamente? Liste os critérios -->
- [ ] O comando de ataque engatilha a animação correta sem travar a movimentação indesejadamente.
- [ ] A área de dano (hitbox) é ativada apenas durante os frames corretos do ataque.
- [ ] Há um tempo de recarga (cooldown) para evitar que o jogador abuse do botão de ataque (spam de cliques).

### 🎨 Mockups / Referências
<!-- Se aplicável, adicione esboços, prints de referência ou links de inspiração -->
*Esquema visual da hitbox de ataque em arco na frente do personagem.*

### 🔍 Contexto Adicional
<!-- Alguma outra informação que ajude a entender a solicitação? -->
Gatilhos visuais (como partículas) ou sonoros ao golpear são altamente recomendados para dar melhor feedback ao jogador.
