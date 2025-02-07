## _VERSION_ _DATE_
* Se facilitó la visualización de grupos de preguntas/opciones y la inclusión y exclusión de los mismos en conjunto.
* Se añadió soporte para los localizadores que necesitan omitir ciertas preguntas, comentarios, respuestas y alternativas.

## 3.1.20 9 de julio de 2024
* Se añadió una opción para omitir los versículos bíblicos antes de cada pregunta en el guion generado.
* Se añadieron alternativas a varias preguntas, se incorporaron algunas preguntas nuevas y se realizaron pequeñas modificaciones (principalmente correcciones de puntuación y ortografía) en respuestas y notas.
* Se actualizó la localización al inglés británico.

## 3.1.3 - 6 de agosto de 2022

* Internacionalizó el sistema de ayuda y lo tradujo al español. (Las localizaciones adicionales se pueden hacer usando [crowdin](https://crowdin.com/project/transcelerator).)

## 3.1.0 - 3 de marzo de 2022

* Se añadió la capacidad de controlar el tamaño del texto en la cuadrícula principal.

## 3.0.0 - 17 de noviembre de 2021

* Cambio para usar las nuevas interfaces compatibles con Paratext 9.2.
* Esta versión también corrige varias deficiencias, incluyendo problemas de rendimiento al ordenar en la columna de traducción.
* Mejoras en el cuadro de diálogo Añadir pregunta
* Improved interaction with biblical terms
* Maneja adecuadamente los proyectos de solo lectura
* Incluye algunas preguntas nuevas (principalmente en el Antiguo Testamento)

## 2.0.5 - 22 de enero de 2021

* Changed verse numbers to output using a span element with class "verse" so they can be formatted using styles in word processing programs (e.g., MS Word) that recognize the span elements that have the class attribute set. Note: If you use an external CSS, you will need to regenerate it or manually edit it in order for verse numbers to display as superscripted in the generated HTML checking script files. (To regenerate the external CSS, in the **Generate Checking Script** dialog box on the **Appearance** tab, select the **Overwrite Existing CSS File** option.)

## 2.0.3 - 11 de enero de 2021

* Se añadió el sistema de ayuda
* Added (optional) Edit column

## 2.0.0 - 3 de diciembre de 2020

* Versión de 64-bit de Transcelerator para usar con Paratext 9.1 y posterior.

## 1.5.2 - 27 de octubre de 2020

* Versión mínima necesaria para producir archivos compatibles para importar en Scripture Forge.
* Varias mejoras en el filtrado y modificación de preguntas.

## 1.5.0 - 23 de septiembre de 2020

* Se añadió la capacidad de localizar la interfaz de usuario. (Incluye localización parcial al español.)

## 1.4.18 - 31 de julio de 2020

* Se añadió una opción para mostrar los números de versículo en el guion (Nota: Si utiliza un archivo CSS externo, para que los números de versículo aparezcan en superíndice, deberá permitir que Transcelerator sobrescriba su archivo CSS o editar el existente para agregar verse {vertical-align: super; font-size: .80em; color:DimGray;}.
* Mejoras importantes en el contenido de las preguntas y la generación de guiones.
* Nueva opción para controlar cómo se manejan en el guion las preguntas de detalle fuera de orden.
* Mejoras en el diálogo de Nueva pregunta, incluyendo la capacidad de agregar preguntas fuera de orden de versículos.

## 1.3.17 - 11 de mayo de 2020

* e añadieron preguntas faltantes para Lucas 22-24.

## 1.3.11 - 16 de abril de 2019

* Se añadió soporte para el instalador de Paratext 9.

## 1.3.9 - 9 de enero de 2019

* Se añadieron localizaciones (calidad de borrador) de todas las preguntas, respuestas y notas para francés y español.

## 1.3.1 - 9 de agosto de 2018

* Se añadió soporte para la visualización de preguntas, respuestas y notas en idiomas distintos al inglés.

## 1.2.1 - 15 de septiembre de 2015

* Mejora en la capacidad de agregar, modificar y excluir preguntas, incluyendo la posibilidad de agregar preguntas para versículos que no tienen preguntas existentes. Este tipo de cambios ya no requieren un reinicio inmediato de Transcelerator para cargar y procesar cada cambio, ¡así que el rendimiento ahora es mucho mejor!

## 1.1.5439 15 September 2015

* Se solucionaron problemas del instalador.

## 1.1.5430 19 November 2014

* Actualización de las reglas de vocablos bíblicos basada en los cambios de la lista de vocablos bíblicos incluida con Paratext 7.5.

## 1.1.5175 3 March 2014

* Actualizaciones y correcciones de preguntas, principalmente relacionadas con el contenido de 1 y 2 Reyes.

## 1.1.5164 20 February 2014

* Se solucionaron algunos errores de bloqueo en el cuadro de diálogo de Sustitución de Frases y se mejoró la información de errores. Se eliminó la capacidad de ordenar en columnas en ese cuadro de diálogo ya que las filas están ordenadas.
* Actualización de las reglas de vocablos bíblicos basada en los cambios de la lista de vocablos bíblicos incluida con Paratext.

## 1.1.5154 10 February 2014

* Evitar bloqueos cuando Paratext no carga términos clave.
* Actualización de las reglas de vocablos bíblicos basada en los cambios de la lista de vocablos bíblicos incluida con Paratext.

## 1.1.5149 5 February 2014

* Primera versión públicamente promovida (estable) de Transcelerator.
* El cambio automático de teclado de Transcelerator ahora funciona correctamente con Keyman.
* Transcelerator ahora incluye preguntas para todos los libros del Antiguo Testamento. Aunque no son tan exhaustivas como las preguntas para Génesis y el Nuevo Testamento y no fueron escritas explícitamente con el propósito de comprobación de comprensión, pueden servir como un punto de partida útil. Agradeceríamos cualquier contribución de preguntas adicionales.

## 1.5070 - 4 de diciembre de 2013

* Muchas mejoras en las preguntas y varias correcciones de errores.

## 1.0.1 - 9 de abril de 2013

* Recordar configuraciones del usuario.
* Muchas otras modificaciones para ajustar configuraciones para Paratext y mejorar la interacción con Paratext.
* Habilitar cambio de teclado.
* Se añadieron Salmos y Proverbios a la lista maestra de preguntas.

## 1.0.0 - 28 de marzo 2013

* Primera versión de Transcelerator como complemento de Paratext.
