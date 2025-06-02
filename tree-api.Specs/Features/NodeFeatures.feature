Feature: NodeService
  Бизнес-сценарии для управления узлами

  Scenario: Создание узла под существующим родителем
    Given существует дерево с именем "TestTree"
    And существует узел с именем "ParentNode" в дереве "TestTree"
    When я создаю узел с именем "ChildNode" под "ParentNode" в дереве "TestTree"
    Then узел "ChildNode" должен существовать под "ParentNode" в дереве "TestTree"

  Scenario: Удаление узла
    Given существует дерево с именем "TestTree"
    And существует узел с именем "NodeToDelete" в дереве "TestTree"
    When я удаляю узел "NodeToDelete" в дереве "TestTree"
    Then узел "NodeToDelete" не должен существовать в дереве "TestTree"

  Scenario: Переименование узла
    Given существует дерево с именем "TestTree"
    And существует узел с именем "NodeToRename" в дереве "TestTree"
    When я переименовываю узел "NodeToRename" в "RenamedNode" в дереве "TestTree"
    Then узел "RenamedNode" должен существовать в дереве "TestTree"
    And узел "NodeToRename" не должен существовать в дереве "TestTree"
