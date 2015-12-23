---
title: Home
layout: default
---

#Introduction
Gripe With Roslyn is a proof of concept library for leveraging Roslyn Analyzers to aid day to day coding activities. This project shows how you can make detecting, marking and tracking issues in a codebase a normal part of your QA process without a huge amount of effort.

#Roslyn Analyzers
This collection of rules are examples of scenarios you can check for. They are **not** an indication of best practice, they are merely here to show the sort of QA issues that can be tracked.

{% for roslynanalyzers in site.data.roslynanalyzers %}
<h2>{{ roslynanalyzers.longname }}</h2>
<p>{{ roslynanalyzers.description }}</p>
{% endfor %}
