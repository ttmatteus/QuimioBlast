---
name: 3. Nova Funcionalidade: Usar Item
about: Sugira uma melhoria ou nova feature para o projeto
title: "[FEATURE] Sistema de Consumo e Uso de Itens"
labels: enhancement
assignees: ''
---

### 📋 Resumo da Ideia
<!-- Descreva em uma frase o que você gostaria de ver no projeto -->
Permitir que o jogador utilize itens do inventário (como poções ou consumíveis) para aplicar efeitos ao personagem.

### ⚠️ Problema que Resolve
<!-- Qual dor ou limitação essa feature endereça? -->
O jogador não tem como recuperar vida ou mana durante os desafios, tornando a progressão punitiva e limitando a profundidade estratégica do gameplay.

### 💡 Solução Proposta
<!-- Descreva como você imagina que essa funcionalidade deveria funcionar -->
Ao abrir o inventário ou usar um atalho rápido, o jogador pode selecionar um item consumível. O item deve aplicar seu efeito imediatamente (ex: +20 de Vida) e ser removido da quantidade disponível no inventário.

### 🔄 Alternativas Consideradas
<!-- Existe outra forma de resolver o problema? Por que você prefere a solução proposta? -->
*   **Cura automática fora de combate:** Ajuda, mas tira a agência do jogador de gerenciar recursos durante momentos críticos (como lutas contra chefes).

### ✅ Critérios de Aceitação
<!-- Como saberemos que essa feature foi implementada corretamente? Liste os critérios -->
- [ ] O item consumido aplica o modificador correto nos atributos do jogador.
- [ ] A quantidade do item decrementa corretamente no inventário após o uso (e some se chegar a zero).
- [ ] O item não pode ser usado caso o jogador já esteja com os atributos no limite máximo (opcional/regra de design).

### 🎨 Mockups / Referências
<!-- Se aplicável, adicione esboços, prints de referência ou links de inspiração -->
*Uso por hotbar (teclas 1-5) ou clique duplo dentro da tela de inventário.*

### 🔍 Contexto Adicional
<!-- Alguma outra informação que ajude a entender a solicitação? -->
Necessita que a estrutura básica de inventário e os atributos do jogador já estejam minimamente funcionais.
